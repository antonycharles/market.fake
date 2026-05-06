using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using User.Api.Helpers;
using User.Application.Handlers;
using User.Application.Providers;
using User.Core.Handlers;

namespace User.Api.Configurations
{
    public static class DependenceConfiguration
    {
        public static void AddDependence(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IPasswordProvider, PasswordProvider>();

            builder.Services.AddScoped<IClaimsTransformation, ClaimsTranformer>();
            builder.Services.AddTransient<CustomJwtBearerHandler>();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddScheme<JwtBearerOptions, CustomJwtBearerHandler>(JwtBearerDefaults.AuthenticationScheme, options => { });

            builder.Services.AddTransient<IUserHandler, UserHandler>();
            builder.Services.AddTransient<IUserPhotoHandler, UserPhotoHandler>();
            builder.Services.AddTransient<ITokenHandler, TokenHandler>();
            builder.Services.AddTransient<ITokenKeyHandler, TokenKeyHandler>();
        }
    }
}
