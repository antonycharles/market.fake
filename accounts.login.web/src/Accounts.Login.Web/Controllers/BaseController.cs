using Accounts.Login.Infra.Exceptions;
using Accounts.Login.Infra.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Accounts.Login.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        private readonly AccountsLoginSettings _configuration;
        private const string ErrorTitleKey = "Error.Title";
        private const string ErrorMessageKey = "Error.Message";
        private const string ErrorReturnUrlKey = "Error.ReturnUrl";
        private const string ErrorReturnLabelKey = "Error.ReturnLabel";

        public BaseController(IOptions<AccountsLoginSettings> configuration)
        {
            _configuration = configuration.Value;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewData["FileApiUrlPublic"] = _configuration.FileApiUrlPublic;
            base.OnActionExecuting(context);
        }

        public void AddModelError(ExternalApiException ex)
        {
            if(ex.Errors != null && ex.Errors.Any())
            {
                foreach (var error in ex.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }

        protected IActionResult RedirectToError(
            ILogger logger,
            Exception ex,
            string message,
            string? title = null,
            string? returnAction = null,
            string? returnController = null,
            object? routeValues = null,
            string? returnLabel = null)
        {
            logger.LogError(ex, "An unexpected error occurred while processing the request.");

            TempData[ErrorTitleKey] = title ?? "Algo deu errado";
            TempData[ErrorMessageKey] = message;

            var returnUrl = Url.Action(returnAction ?? "Index", returnController ?? "Home", routeValues);
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                TempData[ErrorReturnUrlKey] = returnUrl;
            }

            if (!string.IsNullOrWhiteSpace(returnLabel))
            {
                TempData[ErrorReturnLabelKey] = returnLabel;
            }

            return RedirectToAction("Error", "Home");
        }

        protected string? GetErrorTitle() => TempData[ErrorTitleKey]?.ToString();

        protected string? GetErrorMessage() => TempData[ErrorMessageKey]?.ToString();

        protected string? GetErrorReturnUrl() => TempData[ErrorReturnUrlKey]?.ToString();

        protected string? GetErrorReturnLabel() => TempData[ErrorReturnLabelKey]?.ToString();
    }
}
