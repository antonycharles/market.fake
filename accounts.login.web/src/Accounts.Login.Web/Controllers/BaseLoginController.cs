using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Accounts.Login.Infra.Repositories.Interfaces;
using Accounts.Login.Infra.Requests;
using Accounts.Login.Infra.Responses;
using Accounts.Login.Infra.Settings;
using Accounts.Login.Web.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Accounts.Login.Web.Controllers
{
    [Route("[controller]")]
    public abstract class BaseLoginController : BaseController
    {
        private readonly IUserAuthorizationRepository _userAuthorizationRepository;
        private readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(5);
        private readonly IDistributedCache _cache;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
        
        public BaseLoginController(
            IOptions<AccountsLoginSettings> configuration,
            IUserAuthorizationRepository userAuthorizationRepository,
            IDistributedCache cache,
            JwtSecurityTokenHandler jwtSecurityTokenHandler) : base(configuration)
        {
            _userAuthorizationRepository = userAuthorizationRepository;
            _cache = cache;
            _jwtSecurityTokenHandler = jwtSecurityTokenHandler;
        }
        
        protected async Task<IActionResult> UserAuthenticationAsync(UserAuthenticationRequest request)
        {
            var result = await _userAuthorizationRepository.AuthenticateAsync(request);
            var userInfo = await _userAuthorizationRepository.GetUserInfoByTokenAsync(result.Token);

            await AddCookieAuthentication(result, userInfo);

            return await GenerateCode(result);
        }

        protected async Task<IActionResult> GenerateCode(AuthenticationResponse result)
        {
            var code = Guid.NewGuid().ToString();

            await _cache.SetStringAsync(
                code,
                JsonSerializer.Serialize(result),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = _cacheExpiry }
            );

            if (result.CallbackUrl != null && result.CallbackUrl != "")
                return Redirect($"{result.CallbackUrl}?code={code}");
            else
                return Redirect(Url.Action("Index", "Home"));
        }

        protected async Task AddCookieAuthentication(AuthenticationResponse auth, UserResponse userInfo)
        {
            var loginWebToken = await _userAuthorizationRepository.RefreshTokenAsync(auth.RefreshToken, appSlug: "accounts-login-web", redirectUri: "");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Sid, userInfo.Id.ToString()),
                new Claim(ClaimTypes.Name, userInfo.Name),
                new Claim(ClaimTypes.Email, userInfo.Email),
                new Claim(CustomClaimTypes.RefreshToken, loginWebToken.RefreshToken),
                new Claim(CustomClaimTypes.Image, userInfo.ImageUrl ?? "")
            };

            var jwt = _jwtSecurityTokenHandler.ReadJwtToken(loginWebToken.Token);
            if (jwt.Payload.TryGetValue(CustomClaimTypes.Roles, out var rolesValue))
            {
                if (rolesValue is string role)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("CookieAuth", principal);
        }
    }
}