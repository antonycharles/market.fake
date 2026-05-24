using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Market.Web;
using Market.Web.Providers;
using Market.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

#if RELEASE
var file = "settings/appsettings.Production.json";
#else
var file = "settings/appsettings.Development.json";
#endif

using var http = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
var stream = await http.GetStreamAsync(file);

builder.Configuration.AddJsonStream(stream);

builder.Services.AddSingleton(builder.Configuration);

builder.Services.AddScoped<LoginWebService>();

builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<MemberService>();


await builder.Build().RunAsync();
