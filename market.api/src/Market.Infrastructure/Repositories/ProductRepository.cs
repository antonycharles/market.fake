using Dapper;
using Market.Domain.Entities;
using Market.Domain.Enums;
using Market.Domain.Interfaces;
using Market.Domain.Responses;
using Market.Infrastructure.Data;

namespace Market.Infrastructure.Repositories
{
    public class ProductRepository : CrudRepository<Product>, IProductRepository
    {
        public ProductRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        protected override string TableName => "Product";
        protected override string SelectColumns => @"""Id"", ""StoreId"", ""Name"", ""Code"", ""Slug"", ""Summary"", ""Description"", ""CreatedAt"", ""UpdatedAt"", ""DeletedAt"", ""Status""";
        protected override string InsertColumns => @"""Id"", ""StoreId"", ""Name"", ""Code"", ""Slug"", ""Summary"", ""Description"", ""CreatedAt"", ""UpdatedAt"", ""Status""";
        protected override string InsertValues => @"@Id, @StoreId, @Name, @Code, @Slug, @Summary, @Description, @CreatedAt, @UpdatedAt, @Status";
        protected override string UpdateAssignments => @"""StoreId"" = @StoreId, ""Name"" = @Name, ""Code"" = @Code, ""Slug"" = @Slug, ""Summary"" = @Summary, ""Description"" = @Description, ""Status"" = @Status, ""UpdatedAt"" = @UpdatedAt";

        public async Task<bool> CodeExistsAsync(int code, Guid? ignoreId = null)
        {
            using var connection = await ConnectionFactory.CreateOpenConnectionAsync();

            const string sql = @"
                SELECT COUNT(1)
                FROM ""Product""
                WHERE ""DeletedAt"" IS NULL
                  AND ""Code"" = @Code
                  AND (@IgnoreId IS NULL OR ""Id"" <> @IgnoreId)";

            return await connection.ExecuteScalarAsync<int>(sql, new { Code = code, IgnoreId = ignoreId }) > 0;
        }

        public async Task<IEnumerable<ProductListItemResponse>> GetPagedListAsync(int pageIndex, int pageSize)
        {
            using var connection = await ConnectionFactory.CreateOpenConnectionAsync();

            const string sql = @"
                SELECT
                    p.""Id"",
                    p.""Name"",
                    p.""Code"",
                    p.""Slug"",
                    p.""Summary"",
                    price.""Id"" AS ""ProductPriceId"",
                    price.""OriginalPrice"",
                    price.""SalePrice"",
                    price.""Currency"",
                    price.""ValidFrom"",
                    price.""ValidTo"",
                    photo.""Id"" AS ""ProductPhotoId"",
                    photo.""FileId"" AS ""PhotoFileId"",
                    photo.""Url"" AS ""PhotoUrl"",
                    photo.""Description"" AS ""PhotoDescription""
                FROM ""Product"" p
                LEFT JOIN LATERAL (
                    SELECT ""Id"", ""OriginalPrice"", ""SalePrice"", ""Currency"", ""ValidFrom"", ""ValidTo""
                    FROM ""ProductPrice""
                    WHERE ""ProductId"" = p.""Id"" AND ""DeletedAt"" IS NULL
                    ORDER BY ""ValidFrom"" DESC, ""CreatedAt"" DESC
                    LIMIT 1
                ) price ON TRUE
                LEFT JOIN LATERAL (
                    SELECT ""Id"", ""FileId"", ""Url"", ""Description""
                    FROM ""ProductPhoto""
                    WHERE ""ProductId"" = p.""Id""
                      AND ""Type"" = @PrincipalType
                      AND ""DeletedAt"" IS NULL
                    ORDER BY ""Order"" ASC, ""CreatedAt"" DESC
                    LIMIT 1
                ) photo ON TRUE
                WHERE p.""DeletedAt"" IS NULL
                ORDER BY p.""CreatedAt"" DESC
                LIMIT @PageSize OFFSET @Offset";

            return await connection.QueryAsync<ProductListItemResponse>(sql, new
            {
                PrincipalType = (int)ProductPhotoEnum.Principal,
                PageSize = NormalizePageSize(pageSize),
                Offset = (NormalizePageIndex(pageIndex) - 1) * NormalizePageSize(pageSize)
            });
        }
    }
}
