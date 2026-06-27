using Accounts.Login.Infra.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Accounts.Login.Web.Controllers
{
    [Route("[controller]")]
    public class ResetPasswordController : BaseController
    {
        private readonly ILogger<ResetPasswordController> _logger;

        public ResetPasswordController(
            ILogger<ResetPasswordController> logger, 
            IOptions<AccountsLoginSettings> configuration) : base(configuration)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}