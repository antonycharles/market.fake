using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Accounts.Login.Infra.Exceptions;
using Accounts.Login.Infra.Repositories;
using Accounts.Login.Infra.Repositories.Interfaces;
using Accounts.Login.Infra.Requests;
using Accounts.Login.Infra.Responses;
using Accounts.Login.Infra.Settings;
using Accounts.Login.Web.Extensions;
using Accounts.Login.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Accounts.Login.Web.Controllers
{
    [Route("[controller]")]
    public class LoginController : BaseController
    {
        private readonly ILogger<LoginController> _logger;
        private readonly IDistributedCache _cache;
        private readonly IUserAuthorizationRepository _userAuthorizationRepository;
        private readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(5);
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

        public LoginController(
            ILogger<LoginController> logger, 
            IUserAuthorizationRepository userAuthorizationRepository,
            IDistributedCache cache,
            JwtSecurityTokenHandler jwtSecurityTokenHandler,
            IOptions<AccountsLoginSettings> configuration) : base(configuration)
        {
            _userAuthorizationRepository = userAuthorizationRepository;
            _logger = logger;
            _cache = cache;
            _jwtSecurityTokenHandler = jwtSecurityTokenHandler;
        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync(string? appSlug = "market-web", string? redirectUrl = "")
        {
            try
            {
                if (User.Identity.IsAuthenticated && User.GetRefreshToken() != null && appSlug != null && appSlug != "")
                {
                    var result = await _userAuthorizationRepository.RefreshTokenAsync(User.GetRefreshToken(), appSlug, redirectUrl);
                    var userInfo = await _userAuthorizationRepository.GetUserInfoByTokenAsync(result.Token);
                    await AddCookieAuthentication(result, userInfo);
                    return await GenerateCode(result);
                }

                return View(new UserAuthenticationRequest
                {
                    AppSlug = appSlug,
                    RedirectUrl = redirectUrl
                });
            }
            catch (ExternalApiException ex)
            {
                base.AddModelError(ex);
            }
            catch (Exception ex)
            {
                return RedirectToError(
                    _logger,
                    ex,
                    "Não foi possível iniciar o processo de autenticação agora.",
                    returnAction: "Index",
                    returnController: "Login",
                    routeValues: new { appSlug },
                    returnLabel: "Voltar para login");
            }

            return View(new UserAuthenticationRequest
            {
                AppSlug = appSlug,
                RedirectUrl = redirectUrl
            });
        }

        [HttpPost]
        public async Task<IActionResult> IndexAsync(UserAuthenticationRequest request){
            try
            {
                if (!ModelState.IsValid)
                    return View(request);
                
                var result = await _userAuthorizationRepository.AuthenticateAsync(request);
                var userInfo = await _userAuthorizationRepository.GetUserInfoByTokenAsync(result.Token);

                await AddCookieAuthentication(result, userInfo);

                return await GenerateCode(result);
            }
            catch(ExternalApiException ex)
            {
                base.AddModelError(ex);
            }
            catch (Exception ex)
            {
                return RedirectToError(
                    _logger,
                    ex,
                    "Não foi possível concluir o login neste momento.",
                    returnAction: "Index",
                    returnController: "Login",
                    routeValues: new { request.AppSlug },
                    returnLabel: "Voltar para login");
            }

            return View(request);
        }

        [HttpGet("token")]
        [EnableCors("AllowBlazor")]
        public async Task<IActionResult> TokenAsync(string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                    return BadRequest("Invalid authentication response.");

                var json = await _cache.GetStringAsync(code);
                if (string.IsNullOrEmpty(json))
                    return BadRequest("Invalid authentication response.");

                var auth = JsonSerializer.Deserialize<AuthenticationResponse>(json);
                if (auth == null)
                    return BadRequest("Invalid authentication response.");

                try
                {
                    await _cache.RemoveAsync(code);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while processing the request.");
                }

                return Ok(auth);
            }
            catch (ExternalApiException ex)
            {
                base.AddModelError(ex);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                return BadRequest(ex.Message);
            }
        }

        private async Task<IActionResult> GenerateCode(AuthenticationResponse result)
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
        
        private async Task AddCookieAuthentication(AuthenticationResponse auth, UserResponse userInfo)
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
