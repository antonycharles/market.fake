using Dapper;
using Market.Domain.Entities;
using Market.Domain.Interfaces;
using Market.Infrastructure.Data;

namespace Market.Infrastructure.Repositories
{
    public class ProductStockRepository : CrudRepository<ProductStock>, IProductStockRepository
    {
        public ProductStockRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        protected override string TableName => "ProductStock";
        protected override string SelectColumns => @"""Id"", ""ProductId"", ""AvailableStock"", ""ReservedStock"", ""SoldStock"", ""CreatedAt"", ""UpdatedAt"", ""DeletedAt"", ""Status""";
        protected override string InsertColumns => @"""Id"", ""ProductId"", ""AvailableStock"", ""ReservedStock"", ""SoldStock"", ""CreatedAt"", ""UpdatedAt"", ""Status""";
        protected override string InsertValues => @"@Id, @ProductId, @AvailableStock, @ReservedStock, @SoldStock, @CreatedAt, @UpdatedAt, @Status";
        protected override string UpdateAssignments => @"""ProductId"" = @ProductId, ""AvailableStock"" = @AvailableStock, ""ReservedStock"" = @ReservedStock, ""SoldStock"" = @SoldStock, ""Status"" = @Status, ""UpdatedAt"" = @UpdatedAt";

        public async Task<bool> ProductIdExistsAsync(Guid productId, Guid? ignoreId = null)
        {
            using var connection = await ConnectionFactory.CreateOpenConnectionAsync();

            const string sql = @"
                SELECT COUNT(1)
                FROM ""ProductStock""
                WHERE ""DeletedAt"" IS NULL
                  AND ""ProductId"" = @ProductId
                  AND (@IgnoreId IS NULL OR ""Id"" <> @IgnoreId)";

            return await connection.ExecuteScalarAsync<int>(sql, new { ProductId = productId, IgnoreId = ignoreId }) > 0;
        }
    }
}
