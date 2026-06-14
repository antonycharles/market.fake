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
                .HasMany(i => i.UserPhotos)
                .WithOne(i => i.User)
                .HasForeignKey(i => i.UserId);

            builder
                .HasMany(i => i.UserAddresses)
                .WithOne(i => i.User)
                .HasForeignKey(i => i.UserId);

            builder
                .HasMany(i => i.UserCreditCards)
                .WithOne(i => i.User)
                .HasForeignKey(i => i.UserId);
        }
    }
}
