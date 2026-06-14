using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using User.Api.Helpers;
using User.Application.Handlers;
using User.Core.Handlers;

namespace User.Api.Configurations
{
    public static class DependenceConfiguration
    {
        public static void AddDependence(this WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<IUserHandler, UserHandler>();
            builder.Services.AddTransient<IUserPhotoHandler, UserPhotoHandler>();
            builder.Services.AddTransient<IUserAddressHandler, UserAddressHandler>();
            builder.Services.AddTransient<IUserCreditCardHandler, UserCreditCardHandler>();
        }
    }
}
