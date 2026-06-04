using Market.Domain.Entities;
using Market.Domain.Interfaces;
using Market.Infrastructure.Data;

namespace Market.Infrastructure.Repositories
{
    public class ProductCategoryRepository : CrudRepository<ProductCategory>, IProductCategoryRepository
    {
        public ProductCategoryRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        protected override string TableName => "ProductCategory";
        protected override string SelectColumns => @"""Id"", ""ProductId"", ""CategoryId"", ""Order"", ""CreatedAt"", ""UpdatedAt"", ""DeletedAt"", ""Status""";
        protected override string InsertColumns => @"""Id"", ""ProductId"", ""CategoryId"", ""Order"", ""CreatedAt"", ""UpdatedAt"", ""Status""";
        protected override string InsertValues => @"@Id, @ProductId, @CategoryId, @Order, @CreatedAt, @UpdatedAt, @Status";
        protected override string UpdateAssignments => @"""ProductId"" = @ProductId, ""CategoryId"" = @CategoryId, ""Order"" = @Order, ""Status"" = @Status, ""UpdatedAt"" = @UpdatedAt";
    }
}
