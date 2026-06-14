using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accounts.Core.Entities;
using Accounts.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Api.Seeds
{
    public class PermissionSeed
    {
        public static void Seeder(AccountsContext context){
            SeederAccounts(context);
            SeederManagement(context);
            SeederMarketApi(context);
            SeederUserApi(context);

            context.SaveChanges();
        }

        private static void SeederUserApi(AccountsContext context)
        {
            var app = context.Apps.AsNoTracking().FirstOrDefault(w => w.Slug == "user-api");

            if (app == null)
                return;

            var permissions = new List<Permission>();
            BasicCRUD(context, app, permissions, "User", "user");
            BasicCRUD(context, app, permissions, "Photo", "photo");
            BasicCRUD(context, app, permissions, "Address", "address");
            BasicCRUD(context, app, permissions, "Credit Card", "credit-card");

            permissions.Add(new Permission { Name = "User Me - list", Role = "user-me-list", AppId = app.Id });
            permissions.Add(new Permission { Name = "User Me - update", Role = "user-me-update", AppId = app.Id });

            permissions.Add(new Permission { Name = "Photo Me - list", Role = "photo-me-list", AppId = app.Id });
            permissions.Add(new Permission { Name = "Photo Me - update", Role = "photo-me-update", AppId = app.Id });
            permissions.Add(new Permission { Name = "Photo Me - delete", Role = "photo-me-delete", AppId = app.Id });

            permissions.Add(new Permission { Name = "Address Me - list", Role = "address-me-list", AppId = app.Id });
            permissions.Add(new Permission { Name = "Address Me - update", Role = "address-me-update", AppId = app.Id });
            permissions.Add(new Permission { Name = "Address Me - delete", Role = "address-me-delete", AppId = app.Id });

            permissions.Add(new Permission { Name = "Credit Card Me - list", Role = "credit-card-me-list", AppId = app.Id });
            permissions.Add(new Permission { Name = "Credit Card Me - update", Role = "credit-card-me-update", AppId = app.Id });
            permissions.Add(new Permission { Name = "Credit Card Me - delete", Role = "credit-card-me-delete", AppId = app.Id });

            var permissionsDb = context.Permissions.AsNoTracking().ToList();

            foreach(var permission in permissions)
            {
                if(!permissionsDb.Any(w => w.Role == permission.Role && w.AppId == permission.AppId))
                    context.Permissions.Add(permission);
            }
        }

        private static void SeederAccounts(AccountsContext context)
        {
            var app = context.Apps.AsNoTracking().FirstOrDefault(w => w.Slug == "accounts-api");

            if (app == null)
                return;

            var permissions = new List<Permission>();
            AddPermissionsAccountsBase(context, app, permissions);

            permissions.Add(new Permission { Name = "User - authorization", Role = "user-authorization", AppId = app.Id });
            permissions.Add(new Permission { Name = "Token - public key", Role = "token-public-key", AppId = app.Id });

            var permissionsDb = context.Permissions.AsNoTracking().ToList();

            foreach (var permission in permissions)
            {
                if (!permissionsDb.Any(w => w.Role == permission.Role && w.AppId == permission.AppId))
                    context.Permissions.Add(permission);
            }
        }

        private static void SeederManagement(AccountsContext context)
        {
            var app = context.Apps.AsNoTracking().FirstOrDefault(w => w.Slug == "accounts-management");

            if (app == null)
                return;

            var permissions = new List<Permission>();
            AddPermissionsAccountsBase(context, app, permissions);

            var permissionsDb = context.Permissions.AsNoTracking().ToList();

            foreach(var permission in permissions)
            {
                if(!permissionsDb.Any(w => w.Role == permission.Role && w.AppId == permission.AppId))
                    context.Permissions.Add(permission);
            }
        }

        private static void AddPermissionsAccountsBase(AccountsContext context, App app, List<Permission> permissions)
        {
            BasicCRUD(context, app, permissions, "App", "app");
            BasicCRUD(context, app, permissions, "Client", "client");
            BasicCRUD(context, app, permissions, "Client profile", "client-profile");
            BasicCRUD(context, app, permissions, "Permission", "permission");
            BasicCRUD(context, app, permissions, "Profile", "profile");
            BasicCRUD(context, app, permissions, "User", "user");
            BasicCRUD(context, app, permissions, "User profile", "user-profile");
        }


        private static void SeederMarketApi(AccountsContext context)
        {
            var app = context.Apps.AsNoTracking().FirstOrDefault(w => w.Slug == "market-api");

            if (app == null)
                return;

            var permissions = new List<Permission>();
            BasicCRUD(context, app, permissions, "Stores", "stores");
            BasicCRUD(context, app, permissions, "Categories", "categories");
            BasicCRUD(context, app, permissions, "Products", "products");
            BasicCRUD(context, app, permissions, "Product Stocks", "product-stocks");
            BasicCRUD(context, app, permissions, "Product Prices", "product-prices");
            BasicCRUD(context, app, permissions, "Product Photos", "product-photos");
            BasicCRUD(context, app, permissions, "Product Informations", "product-informations");
            BasicCRUD(context, app, permissions, "Product Categories", "product-categories");

            var permissionsDb = context.Permissions.AsNoTracking().ToList();

            foreach(var permission in permissions)
            {
                if(!permissionsDb.Any(w => w.Role == permission.Role && w.AppId == permission.AppId))
                    context.Permissions.Add(permission);
            }
        }

        private static void BasicCRUD(AccountsContext context, App app, List<Permission> permissions, string namePermission, string rolePermission)
        {
            var permissionId = context.Permissions.FirstOrDefault(w => w.Role == rolePermission + "-list" && w.AppId == app.Id)?.Id ?? Guid.NewGuid();
            permissions.Add(new Permission { Id = permissionId, Name = namePermission + " - list", Role = rolePermission + "-list", AppId = app.Id });
            permissions.Add(new Permission { Name = namePermission + " - create", Role = rolePermission + "-create", AppId = app.Id, PermissionFatherId = permissionId });
            permissions.Add(new Permission { Name = namePermission + " - update", Role = rolePermission + "-update", AppId = app.Id, PermissionFatherId = permissionId });
            permissions.Add(new Permission { Name = namePermission + " - delete", Role = rolePermission + "-delete", AppId = app.Id, PermissionFatherId = permissionId });
        }
    }
}