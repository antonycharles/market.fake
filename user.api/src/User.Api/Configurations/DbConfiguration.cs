using Microsoft.EntityFrameworkCore;
using User.Core;
using User.Infrastructure.Data;

namespace User.Api.Configurations
{
    public static class DbConfiguration
    {
        public static void AddDataBase(this WebApplicationBuilder builder, UserSettings settings)
        {
            builder.Services.AddDbContext<UserContext>(x =>
                x.UseNpgsql(settings.DatabaseConnection), ServiceLifetime.Singleton);
        }

        public static void AddMigration(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();

            try
            {
                var db = services.GetRequiredService<UserContext>();
                db.Database.EnsureCreated();
            }
            catch (Exception exception)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(exception, "An error occurred creating the DB.");
            }
        }
    }
}
