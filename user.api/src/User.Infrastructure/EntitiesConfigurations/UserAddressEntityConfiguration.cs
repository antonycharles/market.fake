using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using User.Core.Entities;

namespace User.Infrastructure.EntitiesConfigurations
{
    public class UserAddressEntityConfiguration : IEntityTypeConfiguration<UserAddress>
    {
        public void Configure(EntityTypeBuilder<UserAddress> builder)
        {
            builder
                .HasIndex(i => i.UserId);
        }
    }
}
