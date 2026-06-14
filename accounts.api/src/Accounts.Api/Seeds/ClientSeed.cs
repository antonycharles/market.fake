using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accounts.Application.Providers;
using Accounts.Core.Entities;
using Accounts.Core.Enums;
using Accounts.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Api.Seeds
{
    public class ClientSeed
    {
        public static void Seeder(AccountsContext context, IPasswordProvider passwordProvider){
            var clients = new List<Client>();

            clients.Add(new Client{
                Id = new Guid("d3b0f1a2-4c5e-4b8c-9f7e-1a2b3c4d5e6f"),
                Name = "accounts - Login",
                Password = passwordProvider.HashPassword("123456"),
                Status = StatusEnum.Active
            });

            clients.Add(new Client{
                Id = new Guid("e7f8a9b0-1c2d-3e4f-5a6b-7c8d9e0f1a2b"),
                Name = "accounts - Management",
                Password = passwordProvider.HashPassword("123456"),
                Status = StatusEnum.Active
            });


            clients.Add(new Client{
                Id = new Guid("9bc91fc4-e79a-4f68-9b9a-693ddf61a7e3"),
                Name = "file - API",
                Password = passwordProvider.HashPassword("123456"),
                Status = StatusEnum.Active
            });

            clients.Add(new Client{
                Id = new Guid("a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d"),
                Name = "project - API",
                Password = passwordProvider.HashPassword("123456"),
                Status = StatusEnum.Active
            });

            clients.Add(new Client{
                Id = new Guid("c94aa733-fdf9-40eb-82a9-5f6508273a76"),
                Name = "user - API",
                Password = passwordProvider.HashPassword("123456"),
                Status = StatusEnum.Active
            });

            var clientsDb = context.Clients.AsNoTracking().ToList();

            foreach(var client in clients)
            {
                if(!clientsDb.Any(w => w.Id == client.Id))
                    context.Clients.Add(client);
            }

            context.SaveChanges();
        }
    }
}