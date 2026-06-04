using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accounts.Api.Helpers;
using Accounts.Core.Entities;
using Accounts.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Api.Seeds
{
    public class ProfilePermissionSeed
    {
        public static void Seeder(AccountsContext context){
            SeederAccountsApiAdmin(context);
            SeederAccountsApiLogin(context);
            SeederAccountsApiInfo(context);
            SeederAccountsManagementAdmin(context);
            SeederAccountsManagementUser(context);
            SeederAccountsApiPublicToken(context);
            SeederMarketApiAdmin(context);

            context.SaveChanges();
        }

        private static void SeederMarketApiAdmin(AccountsContext context)
        {
            var app = context.Apps.AsNoTracking().FirstOrDefault(w => w.Slug == "market-api");

            if(app == null)
                return;

            var profile = context.Profiles.AsNoTracking()
                .FirstOrDefault(w => w.Slug == "admin" && w.AppId == app.Id);

            if(profile == null)
                return;

            var permissions = context.Permissions.AsNoTracking().Where(w => w.AppId == app.Id).ToList();

            AddProfilePermission(context, profile, permissions);
        }

        private static void SeederAccountsManagementAdmin(AccountsContext context)
        {
            var app = context.Apps.AsNoTracking().FirstOrDefault(w => w.Slug == "accounts-management");

            if(app == null)
                return;

            var profile = context.Profiles.AsNoTracking()
                .FirstOrDefault(w => w.Slug == "admin" && w.AppId == app.Id);

            if(profile == null)
                return;

            var permissions = context.Permissions.AsNoTracking().Where(w => w.AppId == app.Id).ToList();

            AddProfilePermission(context, profile, permissions);
        }

        private static void SeederAccountsManagementUser(AccountsContext context)
        {
            var app = context.Apps.AsNoTracking().FirstOrDefault(w => w.Slug == "accounts-management");

            if(app == null)
                return;

            var profile = context.Profiles.AsNoTracking()
                .FirstOrDefault(w => w.Slug == "user" && w.AppId == app.Id);

            if(profile == null)
                return;

            var roles = new List<string>
            {
                "user-list",
                "user-profile-list"
            };

            var permissions = context.Permissions.AsNoTracking()
                .Where(w => w.AppId == app.Id && roles.Contains(w.Role)).ToList();

            AddProfilePermission(context, profile, permissions);
        }

        private static void SeederAccountsApiLogin(AccountsContext context)
        {
            var app = context.Apps.AsNoTracking().FirstOrDefault(w => w.Slug == "accounts-api");

            if(app == null)
                return;

            var profile = context.Profiles.AsNoTracking()
                .FirstOrDefault(w => w.Slug == "login" && w.AppId == app.Id);

            if(profile == null)
                return;

            var roles = new List<string>
            {
                "user-authorization",
                "token-public-key",
                "user-create",
                "user-update",
                "app-list"
            };

            var permissions = context.Permissions.AsNoTracking()
                .Where(w => w.AppId == app.Id && roles.Contains(w.Role)).ToList();

            AddProfilePermission(context, profile, permissions);
        }

        private static void SeederAccountsApiInfo(AccountsContext context)
        {
            var app = context.Apps.AsNoTracking().FirstOrDefault(w => w.Slug == "accounts-api");

            if(app == null)
                return;

            var profile = context.Profiles.AsNoTracking()
                .FirstOrDefault(w => w.Slug == "info" && w.AppId == app.Id);

            if(profile == null)
                return;

            var roles = new List<string>
            {
                "app-list",
                "user-list",
                "profile-list",
            };

            var permissions = context.Permissions.AsNoTracking()
                .Where(w => w.AppId == app.Id && roles.Contains(w.Role)).ToList();

            AddProfilePermission(context, profile, permissions);
        }

        private static void SeederAccountsApiPublicToken(AccountsContext context)
        {
            var app = context.Apps.AsNoTracking().FirstOrDefault(w => w.Slug == "accounts-api");

            if(app == null)
                return;

            var profile = context.Profiles.AsNoTracking()
                .FirstOrDefault(w => w.Slug == "public-token" && w.AppId == app.Id);

            if(profile == null)
                return;

            var roles = new List<string>
            {
                "token-public-key"
            };

            var permissions = context.Permissions.AsNoTracking()
                .Where(w => w.AppId == app.Id && roles.Contains(w.Role)).ToList();

            AddProfilePermission(context, profile, permissions);
        }

        private static void SeederAccountsApiAdmin(AccountsContext context)
        {
            var app = context.Apps.AsNoTracking().FirstOrDefault(w => w.Slug == "accounts-api");

            if (app == null)
                return;

            var profile = context.Profiles.AsNoTracking().FirstOrDefault(w => w.Slug == "admin" && w.AppId == app.Id);

            if (profile == null)
                return;

            var rolesIgnore = new List<string>
            {
                "user-authorization",
            };

            var permissions = context.Permissions.AsNoTracking()
                .Where(w => w.AppId == app.Id && rolesIgnore.Contains(w.Role) == false)
                .ToList();

            AddProfilePermission(context, profile, permissions);
        }


        private static void AddProfilePermission(AccountsContext context, Profile profile, List<Permission> permissions)
        {
            foreach (var permission in permissions)
            {
                var profilePermission = context.ProfilePermissions.AsNoTracking()
                    .FirstOrDefault(w => w.ProfileId == profile.Id && w.PermissionId == permission.Id);

                if (profilePermission == null)
                    context.ProfilePermissions.Add(new ProfilePermission { ProfileId = profile.Id, PermissionId = permission.Id });
            }
        }
    }
}