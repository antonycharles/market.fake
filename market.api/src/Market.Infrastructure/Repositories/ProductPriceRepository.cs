using Dapper;
using Market.Domain.Entities;
using Market.Domain.Interfaces;
using Market.Infrastructure.Data;

namespace Market.Infrastructure.Repositories
{
    public class ProductPriceRepository : CrudRepository<ProductPrice>, IProductPriceRepository
    {
        public ProductPriceRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        protected override string TableName => "ProductPrice";
        protected override string SelectColumns => @"""Id"", ""ProductId"", ""OriginalPrice"", ""SalePrice"", ""Currency"", ""ValidFrom"", ""ValidTo"", ""CreatedAt"", ""UpdatedAt"", ""DeletedAt"", ""Status""";
        protected override string InsertColumns => @"""Id"", ""ProductId"", ""OriginalPrice"", ""SalePrice"", ""Currency"", ""ValidFrom"", ""ValidTo"", ""CreatedAt"", ""UpdatedAt"", ""Status""";
        protected override string InsertValues => @"@Id, @ProductId, @OriginalPrice, @SalePrice, @Currency, @ValidFrom, @ValidTo, @CreatedAt, @UpdatedAt, @Status";
        protected override string UpdateAssignments => @"""ProductId"" = @ProductId, ""OriginalPrice"" = @OriginalPrice, ""SalePrice"" = @SalePrice, ""Currency"" = @Currency, ""ValidFrom"" = @ValidFrom, ""ValidTo"" = @ValidTo, ""Status"" = @Status, ""UpdatedAt"" = @UpdatedAt";

        public async Task<bool> HasOverlappingIntervalAsync(Guid productId, DateTime validFrom, DateTime? validTo, Guid? ignoreId = null)
        {
            using var connection = await ConnectionFactory.CreateOpenConnectionAsync();

            const string sql = @"
                SELECT COUNT(1)
                FROM ""ProductPrice""
                WHERE ""DeletedAt"" IS NULL
                  AND ""ProductId"" = @ProductId
                  AND (@IgnoreId IS NULL OR ""Id"" <> @IgnoreId)
                  AND @ValidFrom <= COALESCE(""ValidTo"", 'infinity'::timestamp)
                  AND COALESCE(CAST(@ValidTo AS timestamp), 'infinity'::timestamp) >= ""ValidFrom""";

            return await connection.ExecuteScalarAsync<int>(sql, new
            {
                ProductId = productId,
                ValidFrom = validFrom,
                ValidTo = validTo,
                IgnoreId = ignoreId
            }) > 0;
        }
    }
}
