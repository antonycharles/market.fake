using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accounts.Core;
using Accounts.Core.Entities;
using Accounts.Core.Enums;
using Accounts.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Api.Seeds
{
    public class AppSeed
    {
        public static void Seeder(AccountsContext context, AccountsSettings settings){

            var apps = new List<App>();
            
            apps.Add(new App{
                Code = 1,
                Type = AppTypeEnum.Api,
                Name = "Accounts - Api",
                Slug = "accounts-api",
                Status = StatusEnum.Active
            });

            apps.Add(new App{
                Code = 2,
                Type = AppTypeEnum.Api,
                Name = "Money - Web",
                Slug = "money-api",
                IsPublic = true,
                Status = StatusEnum.Active
            });

            apps.Add(new App{
                Code = 3,
                Type = AppTypeEnum.Api,
                Name = "Task - Web",
                Slug = "task-api",
                IsPublic = true,
                Status = StatusEnum.Active
            });
            
            apps.Add(new App{
                Code = 4,
                Type = AppTypeEnum.Web,
                Name = "Accounts - Management",
                Slug = "accounts-management",
                FaviconUrl = $"{settings.FileApiUrl}/File/d8e6b464-3b70-42f3-8bd4-7f77c8ca5189",
                IsPublic = true,
                Status = StatusEnum.Active
            });

            apps.Add(new App
            {
                Code = 5,
                Type = AppTypeEnum.Api,
                Name = "File - API",
                Slug = "file-api",
                Status = StatusEnum.Active
            });


            apps.Add(new App
            {
                Code = 6,
                Type = AppTypeEnum.Api,
                Name = "Notification - Api",
                Slug = "notification-api",
                Status = StatusEnum.Active
            });
            
            apps.Add(new App{
                Code = 7,
                Type = AppTypeEnum.Api,
                Name = "Market - Web",
                Slug = "market-api",
                FaviconUrl = $"{settings.FileApiUrl}/File/71402371-2fc0-4e15-b9e0-384f45508afb",
                IsPublic = true,
                Status = StatusEnum.Active
            });

            apps.Add(new App{
                Code = 8,
                Type = AppTypeEnum.Web,
                Name = "Accounts Login - Web",
                Slug = "accounts-login-web",
                Status = StatusEnum.Active
            });

            var appsDb = context.Apps.AsNoTracking().ToList();

            foreach(var app in apps)
            {
                if(!appsDb.Any(w => w.Slug == app.Slug))
                    context.Apps.Add(app);
            }

            context.SaveChanges();
        }
    }
}