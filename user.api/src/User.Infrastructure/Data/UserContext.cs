using Microsoft.EntityFrameworkCore;
using User.Core.Entities;
using User.Infrastructure.EntitiesConfigurations;
using UserEntity = User.Core.Entities.User;

namespace User.Infrastructure.Data
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserEntityConfiguration).Assembly);
        }

        public virtual DbSet<UserEntity> Users { get; set; }
        public virtual DbSet<UserPhoto> UserPhotos { get; set; }
    }
}
