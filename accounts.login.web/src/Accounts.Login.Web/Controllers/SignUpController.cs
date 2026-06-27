using System.IdentityModel.Tokens.Jwt;
using Accounts.Login.Infra.Exceptions;
using Accounts.Login.Infra.Repositories.Interfaces;
using Accounts.Login.Infra.Requests;
using Accounts.Login.Infra.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace Accounts.Login.Web.Controllers
{
    [Route("[controller]")]
    public class SignUpController : BaseLoginController
    {
        private readonly ILogger<SignUpController> _logger;
        private readonly IUserRepository _userRepository;

        public SignUpController(
            ILogger<SignUpController> logger, 
            IUserRepository userRepository,
            IOptions<AccountsLoginSettings> configuration,
            IUserAuthorizationRepository userAuthorizationRepository,
            IDistributedCache cache,
            JwtSecurityTokenHandler jwtSecurityTokenHandler) : base(configuration,
                userAuthorizationRepository,
                cache,
                jwtSecurityTokenHandler)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index(string? AppSlug = "")
        {
            return View(new UserRequest
            {
                AppSlug = AppSlug
            });
        }

        [HttpPost]
        public async Task<IActionResult> Index(UserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(request);

                var user = await _userRepository.CreateAsync(request);
                
                return await base.UserAuthenticationAsync(new UserAuthenticationRequest{
                    AppSlug = request.AppSlug ?? "market-web",
                    Email = request.Email,
                    Password = request.Password
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
                    "Não foi possível concluir seu cadastro neste momento.",
                    returnAction: "Index",
                    returnController: "SignUp",
                    routeValues: new { request.AppSlug },
                    returnLabel: "Voltar para cadastro");
            }

            return View(request);
        }
    }
}
