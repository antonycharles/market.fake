using Accounts.Login.Infra.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Accounts.Login.Web.Controllers
{
    [Route("[controller]")]
    public class LogoutController : BaseController
    {
        private readonly ILogger<LogoutController> _logger;

        public LogoutController(
            ILogger<LogoutController> logger,
            IOptions<AccountsLoginSettings> configuration) : base(configuration)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            try
            {
                if(!User.Identity.IsAuthenticated)
                    return RedirectToAction("Index", "Login");

                await HttpContext.SignOutAsync("CookieAuth");

                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                return RedirectToError(
                    _logger,
                    ex,
                    "Não foi possível encerrar sua sessão agora.",
                    returnAction: "Index",
                    returnController: "Home",
                    returnLabel: "Voltar para home");
            }
        }
    }
}
