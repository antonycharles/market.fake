using Dapper;
using Market.Domain.Entities;
using Market.Domain.Interfaces;
using Market.Infrastructure.Data;

namespace Market.Infrastructure.Repositories
{
    public class ProductInformationRepository : CrudRepository<ProductInformation>, IProductInformationRepository
    {
        public ProductInformationRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        protected override string TableName => "ProductInformation";
        protected override string SelectColumns => @"""Id"", ""ProductId"", ""Type"", ""Label"", ""Value"", ""Order"", ""CreatedAt"", ""UpdatedAt"", ""DeletedAt"", ""Status""";
        protected override string InsertColumns => @"""Id"", ""ProductId"", ""Type"", ""Label"", ""Value"", ""Order"", ""CreatedAt"", ""UpdatedAt"", ""Status""";
        protected override string InsertValues => @"@Id, @ProductId, @Type, @Label, @Value, @Order, @CreatedAt, @UpdatedAt, @Status";
        protected override string UpdateAssignments => @"""ProductId"" = @ProductId, ""Type"" = @Type, ""Label"" = @Label, ""Value"" = @Value, ""Order"" = @Order, ""Status"" = @Status, ""UpdatedAt"" = @UpdatedAt";

        public async Task<IEnumerable<ProductInformation>> GetByProductIdAsync(Guid productId)
        {
            using var connection = await ConnectionFactory.CreateOpenConnectionAsync();

            var sql = $@"
                SELECT {SelectColumns}
                FROM ""{TableName}""
                WHERE ""DeletedAt"" IS NULL
                  AND ""ProductId"" = @ProductId
                ORDER BY ""Order"", ""CreatedAt""";

            return await connection.QueryAsync<ProductInformation>(sql, new { ProductId = productId });
        }
    }
}
