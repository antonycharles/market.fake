using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using User.Core.Entities;

namespace User.Infrastructure.EntitiesConfigurations
{
    public class UserCreditCardEntityConfiguration : IEntityTypeConfiguration<UserCreditCard>
    {
        public void Configure(EntityTypeBuilder<UserCreditCard> builder)
        {
            builder
                .HasIndex(i => i.UserId);
        }
    }
}
