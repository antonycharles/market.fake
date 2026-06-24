using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Accounts.Login.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Accounts.Login.Infra.Repositories.Interfaces;
using Accounts.Login.Web.Extensions;
using Accounts.Login.Infra.Settings;
using Microsoft.Extensions.Options;
using Accounts.Login.Web.Helpers;

namespace Accounts.Login.Web.Controllers;

[Authorize]
public class HomeController : BaseHomeController
{
    private readonly ILogger<HomeController> _logger;
    private readonly IAppRepository _appRepository;

    public HomeController(
        ILogger<HomeController> logger, 
        IAppRepository appRepository, 
        IOptions<AccountsLoginSettings> configuration) : base(configuration)
    {
        _logger = logger;
        _appRepository = appRepository;
    }

    public async Task<IActionResult> IndexAsync()
    {
        try
        {
            if(User.IsInRole(RoleConstants.HomeRole.Show) == false)
            {
                return RedirectToAction("Index", "Login", new { appSlug = "market-web" });
            }

            var appsPublic = await _appRepository.GetPublicAppsByUserIdAsync(User.GetId());
            return View(new HomeViewModel {
                Apps = appsPublic.Items.ToList()
            });
        }
        catch (Exception ex)
        {
            return RedirectToError(
                _logger,
                ex,
                "Não foi possível carregar os aplicativos disponíveis para sua conta.",
                returnAction: "Index",
                returnController: "Login",
                returnLabel: "Ir para login");
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [AllowAnonymous]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            Title = GetErrorTitle() ?? "Algo deu errado",
            Message = GetErrorMessage() ?? "Ocorreu um erro inesperado ao processar sua solicitação.",
            ReturnUrl = GetErrorReturnUrl() ?? Url.Action("Index", User.Identity?.IsAuthenticated == true ? "Home" : "Login") ?? "/",
            ReturnLabel = GetErrorReturnLabel() ?? "Tentar novamente"
        });
    }
}
