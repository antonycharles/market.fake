using User.Core;

namespace User.Api.Configurations
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
                .AddOptions<UserSettings>()
                .BindConfiguration(nameof(UserSettings))
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }

        public static UserSettings GetSettings(this WebApplicationBuilder builder)
        {
            return builder.Configuration
                .GetSection(nameof(UserSettings))
                .Get<UserSettings>();
        }
    }
}
