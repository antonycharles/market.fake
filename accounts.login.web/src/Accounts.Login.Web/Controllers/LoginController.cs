using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Accounts.Login.Infra.Exceptions;
using Accounts.Login.Infra.Repositories.Interfaces;
using Accounts.Login.Infra.Requests;
using Accounts.Login.Infra.Responses;
using Accounts.Login.Infra.Settings;
using Accounts.Login.Web.Extensions;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace Accounts.Login.Web.Controllers
{
    [Route("[controller]")]
    public class LoginController : BaseLoginController
    {
        private readonly ILogger<LoginController> _logger;
        private readonly IUserAuthorizationRepository _userAuthorizationRepository;
        private readonly IDistributedCache _cache;

        public LoginController(
            ILogger<LoginController> logger, 
            IUserAuthorizationRepository userAuthorizationRepository,
            IDistributedCache cache,
            JwtSecurityTokenHandler jwtSecurityTokenHandler,
            IOptions<AccountsLoginSettings> configuration) : base(
                configuration, 
                userAuthorizationRepository,
                cache,
                jwtSecurityTokenHandler)
        {
            _userAuthorizationRepository = userAuthorizationRepository;
            _logger = logger;
            _cache = cache;
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
                    await base.AddCookieAuthentication(result, userInfo);
                    return await base.GenerateCode(result);
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
                
                return await base.UserAuthenticationAsync(request);
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

        
        
        
    }
}
