using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accounts.Core.Entities;
using Accounts.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Api.Seeds
{
    public class ProfileSeed
    {

        public static void Seeder(AccountsContext context)
        {
            SeederAccountsApi(context);
            SeederAccountsManagement(context);
            SeederFileApi(context);
            SeederMarketApi(context);
            SeederAccountsLoginWeb(context);

            context.SaveChanges();
        }

        private static void SeederAccountsLoginWeb(AccountsContext context)
        {
            var app = context.Apps.AsNoTracking().FirstOrDefault(w => w.Slug == "accounts-login-web");

            if (app == null)
                return;

            var profiles = new List<Profile>();

            profiles.Add(new Profile { Name = "User", AppId = app.Id, Slug = "user", IsDefault = true });

            var profilesDb = context.Profiles.AsNoTracking().Where(w => w.AppId == app.Id).ToList();

            foreach (var profile in profiles)
            {
                if (!profilesDb.Any(w => w.Slug == profile.Slug))
                    context.Profiles.Add(profile);
            }
        }

        private static void SeederAccountsApi(AccountsContext context)
        {
            var app = context.Apps.AsNoTracking().FirstOrDefault(w => w.Slug == "accounts-api");

            if (app == null)
                return;

            var profiles = new List<Profile>();

            profiles.Add(new Profile { Name = "Admin", AppId = app.Id, Slug = "admin", Type = Core.Enums.ProfileTypeEnum.System });
            profiles.Add(new Profile { Name = "Login", AppId = app.Id, Slug = "login", Type = Core.Enums.ProfileTypeEnum.System });
            profiles.Add(new Profile { Name = "Public token", AppId = app.Id, Slug = "public-token", Type = Core.Enums.ProfileTypeEnum.System });
            profiles.Add(new Profile { Name = "Info", AppId = app.Id, Slug = "info", Type = Core.Enums.ProfileTypeEnum.System });

            var profilesDb = context.Profiles.AsNoTracking().Where(w => w.AppId == app.Id).ToList();

            foreach (var profile in profiles)
            {
                if (!profilesDb.Any(w => w.Slug == profile.Slug))
                    context.Profiles.Add(profile);
            }
        }

        private static void SeederAccountsManagement(AccountsContext context)
        {
            var app = context.Apps.AsNoTracking().FirstOrDefault(w => w.Slug == "accounts-management");

            if (app == null)
                return;

            var profiles = new List<Profile>();

            profiles.Add(new Profile { Name = "Admin", AppId = app.Id, Slug = "admin" });
            profiles.Add(new Profile { Name = "User", AppId = app.Id, Slug = "user", IsDefault = true });

            var profilesDb = context.Profiles.AsNoTracking().Where(w => w.AppId == app.Id).ToList();

            foreach (var profile in profiles)
            {
                if (!profilesDb.Any(w => w.Slug == profile.Slug))
                    context.Profiles.Add(profile);
            }
        }
        
        private static void SeederFileApi(AccountsContext context)
        {
            var app = context.Apps.AsNoTracking().FirstOrDefault(w => w.Slug == "file-api");

            if(app == null)
                return;

            var profiles = new List<Profile>();

            profiles.Add(new Profile { Name = "Public", AppId = app.Id, Slug = "public" });

            var profilesDb = context.Profiles.AsNoTracking().Where(w => w.AppId == app.Id).ToList();

            foreach(var profile in profiles)
            {
                if(!profilesDb.Any(w => w.Slug == profile.Slug))
                    context.Profiles.Add(profile);
            }
        }

        private static void SeederMarketApi(AccountsContext context)
        {
            var app = context.Apps.AsNoTracking().FirstOrDefault(w => w.Slug == "market-api");

            if(app == null)
                return;

            var profiles = new List<Profile>();

            profiles.Add(new Profile { Name = "User", AppId = app.Id, Slug = "user", IsDefault = true });
            profiles.Add(new Profile { Name = "Admin", AppId = app.Id, Slug = "admin", IsDefault = false });

            var profilesDb = context.Profiles.AsNoTracking().Where(w => w.AppId == app.Id).ToList();

            foreach(var profile in profiles)
            {
                if(!profilesDb.Any(w => w.Slug == profile.Slug))
                    context.Profiles.Add(profile);
            }
        }
        
    }
}