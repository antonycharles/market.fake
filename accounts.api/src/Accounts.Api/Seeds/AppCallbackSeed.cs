using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accounts.Core.Entities;
using Accounts.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Api.Seeds
{
    public class AppCallbackSeed
    {
        public static void Seeder(AccountsContext context)
        {
            var apps = context.Apps.ToList();

            var callbacks = new List<AppCallback>();
            
            callbacks.Add(new AppCallback{
                Url = "http://localhost/accounts-management/Account/Callback",
                Environment = Core.Enums.EnvironmentEnum.Staging,
                AppId = apps.FirstOrDefault(a => a.Slug == "accounts-management").Id,
                IsDefault = true
            });

            callbacks.Add(new AppCallback{
                Url = "http://localhost/accounts",
                Environment = Core.Enums.EnvironmentEnum.Staging,
                AppId = apps.FirstOrDefault(a => a.Slug == "accounts-login-web").Id,
                IsDefault = true
            });


            callbacks.Add(new AppCallback{
                Url = "http://localhost/market/auth/callback",
                Environment = Core.Enums.EnvironmentEnum.Staging,
                AppId = apps.FirstOrDefault(a => a.Slug == "market-web").Id,
                IsDefault = true
            });

            var callbacksDb = context.AppCallbacks.AsNoTracking().ToList();

            foreach(var callback in callbacks)
            {
                if(!callbacksDb.Any(w => w.Url == callback.Url))
                    context.AppCallbacks.Add(callback);
            }

            context.SaveChanges();
        }
    }
}