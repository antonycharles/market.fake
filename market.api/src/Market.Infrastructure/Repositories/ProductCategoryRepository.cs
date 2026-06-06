using Dapper;
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

        public async Task<IEnumerable<Category>> GetCategoriesByProductIdAsync(Guid productId)
        {
            using var connection = await ConnectionFactory.CreateOpenConnectionAsync();

            const string sql = @"
                SELECT c.""Id"", c.""Name"", c.""Slug"", c.""Description"", c.""CreatedAt"", c.""UpdatedAt"", c.""DeletedAt"", c.""Status""
                FROM ""ProductCategory"" pc
                INNER JOIN ""Category"" c ON c.""Id"" = pc.""CategoryId""
                WHERE pc.""DeletedAt"" IS NULL
                  AND c.""DeletedAt"" IS NULL
                  AND pc.""ProductId"" = @ProductId
                ORDER BY pc.""Order"", c.""Name""";

            return await connection.QueryAsync<Category>(sql, new { ProductId = productId });
        }
    }
}
