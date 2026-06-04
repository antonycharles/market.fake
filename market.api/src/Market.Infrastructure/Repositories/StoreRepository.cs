using Market.Domain.Entities;
using Market.Domain.Interfaces;
using Market.Infrastructure.Data;

namespace Market.Infrastructure.Repositories
{
    public class StoreRepository : CrudRepository<Store>, IStoreRepository
    {
        public StoreRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        protected override string TableName => "Store";
        protected override string SelectColumns => @"""Id"", ""Name"", ""Description"", ""UserCreatedId"", ""CreatedAt"", ""UpdatedAt"", ""DeletedAt"", ""Status""";
        protected override string InsertColumns => @"""Id"", ""Name"", ""Description"", ""UserCreatedId"", ""CreatedAt"", ""UpdatedAt"", ""Status""";
        protected override string InsertValues => @"@Id, @Name, @Description, @UserCreatedId, @CreatedAt, @UpdatedAt, @Status";
        protected override string UpdateAssignments => @"""Name"" = @Name, ""Description"" = @Description, ""UserCreatedId"" = @UserCreatedId, ""Status"" = @Status, ""UpdatedAt"" = @UpdatedAt";
    }
}
