using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Market.Domain.Settings;

namespace Market.Api.Configurations
{
    public static class ConfigurationRoot
    {
        public static void AddConfigurationRoot(this WebApplicationBuilder builder)
        {
            #if RELEASE
                builder.Configuration.AddJsonFile("secrets/appsettings.json", false, true);
            #else
                builder.Configuration.AddJsonFile("appsettings.json", false, true);
            #endif

            builder.Configuration.AddEnvironmentVariables();

            builder.Services
                .AddOptions<ProjectSettings>()
                .BindConfiguration(nameof(ProjectSettings))
                .ValidateDataAnnotations()
                .ValidateOnStart(); 
        }

        public static ProjectSettings GetSettings(this WebApplicationBuilder builder)
        {
            return builder.Configuration
                .GetSection(nameof(ProjectSettings))
                .Get<ProjectSettings>();
        }
    }
}