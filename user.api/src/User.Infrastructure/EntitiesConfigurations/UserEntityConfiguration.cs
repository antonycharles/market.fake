using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserEntity = User.Core.Entities.User;

namespace User.Infrastructure.EntitiesConfigurations
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder
                .HasIndex(i => i.Email)
                .IsUnique();

            builder
                .HasOne(i => i.UserPhoto)
                .WithOne(i => i.User)
                .HasForeignKey<User.Core.Entities.UserPhoto>(i => i.UserId);
        }
    }
}
