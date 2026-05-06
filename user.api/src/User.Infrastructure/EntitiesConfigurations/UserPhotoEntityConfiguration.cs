using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using User.Core.Entities;

namespace User.Infrastructure.EntitiesConfigurations
{
    public class UserPhotoEntityConfiguration : IEntityTypeConfiguration<UserPhoto>
    {
        public void Configure(EntityTypeBuilder<UserPhoto> builder)
        {
            builder
                .HasIndex(i => i.UserId)
                .IsUnique();
        }
    }
}
