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

        public async Task<Product?> GetByCodeAsync(int code)
        {
            using var connection = await ConnectionFactory.CreateOpenConnectionAsync();

            var sql = $@"
                SELECT {SelectColumns}
                FROM ""Product""
                WHERE ""DeletedAt"" IS NULL
                  AND ""Code"" = @Code";

            return await connection.QueryFirstOrDefaultAsync<Product>(sql, new { Code = code });
        }

        public async Task<IEnumerable<ProductListItemResponse>> GetPagedListAsync(int pageIndex, int pageSize, Guid? categoryId = null, string? search = null, ProductOrderEnum? order = null)
        {
            using var connection = await ConnectionFactory.CreateOpenConnectionAsync();

            string sql = @"
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
                    photo.""Description"" AS ""PhotoDescription"",
                    stock.""AvailableStock"" AS ""AvailableStock"",
                    stock.""ReservedStock"" AS ""ReservedStock"",
                    stock.""SoldStock"" AS ""SoldStock""
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
                LEFT JOIN LATERAL (
                    SELECT ""Id"", ""AvailableStock"", ""ReservedStock"", ""SoldStock""
                    FROM ""ProductStock""
                    WHERE ""ProductId"" = p.""Id""
                      AND ""DeletedAt"" IS NULL
                    ORDER BY ""CreatedAt"" DESC
                    LIMIT 1
                ) stock ON TRUE
                WHERE p.""DeletedAt"" IS NULL
                  AND (
                      @CategoryId IS NULL
                      OR EXISTS (
                          SELECT 1
                          FROM ""ProductCategory"" pc
                          WHERE pc.""ProductId"" = p.""Id""
                            AND pc.""CategoryId"" = @CategoryId
                            AND pc.""DeletedAt"" IS NULL
                      )
                  )
                  AND (
                      @Search IS NULL
                      OR p.""Name"" ILIKE @SearchPattern
                      OR p.""Slug"" ILIKE @SearchPattern
                      OR p.""Summary"" ILIKE @SearchPattern
                      OR p.""Description"" ILIKE @SearchPattern
                      OR CAST(p.""Code"" AS TEXT) ILIKE @SearchPattern
                      OR EXISTS (
                          SELECT 1
                          FROM ""ProductCategory"" pc
                          INNER JOIN ""Category"" c ON c.""Id"" = pc.""CategoryId""
                          WHERE pc.""ProductId"" = p.""Id""
                            AND pc.""DeletedAt"" IS NULL
                            AND c.""DeletedAt"" IS NULL
                            AND (c.""Name"" ILIKE @SearchPattern OR c.""Slug"" ILIKE @SearchPattern)
                      )
                  ) ";

            sql = SetOrder(order, sql);

            sql += @"LIMIT @PageSize OFFSET @Offset";

            return await connection.QueryAsync<ProductListItemResponse>(sql, new
            {
                PrincipalType = (int)ProductPhotoEnum.Principal,
                CategoryId = categoryId,
                Search = NormalizeSearch(search),
                SearchPattern = BuildSearchPattern(search),
                PageSize = NormalizePageSize(pageSize),
                Offset = (NormalizePageIndex(pageIndex) - 1) * NormalizePageSize(pageSize)
            });
        }

        private static string SetOrder(ProductOrderEnum? order, string sql)
        {
            if (order.HasValue)
            {
                sql += "ORDER BY ";

                switch (order.Value)
                {
                    case ProductOrderEnum.NameAsc:
                        sql += @"p.""Name"" ASC ";
                        break;
                    case ProductOrderEnum.NameDesc:
                        sql += @"p.""Name"" DESC ";
                        break;
                    case ProductOrderEnum.CreatedAtAsc:
                        sql += @"p.""CreatedAt"" ASC ";
                        break;
                    case ProductOrderEnum.CreatedAtDesc:
                        sql += @"p.""CreatedAt"" DESC ";
                        break;
                    case ProductOrderEnum.PriceAsc:
                        sql += @"price.""SalePrice"" ASC NULLS LAST ";
                        break;
                    case ProductOrderEnum.PriceDesc:
                        sql += @"price.""SalePrice"" DESC NULLS LAST ";
                        break;
                    case ProductOrderEnum.BestSellingAsc:
                        sql += @"stock.""SoldStock"" ASC NULLS LAST ";
                        break;
                    case ProductOrderEnum.BestSellingDesc:
                        sql += @"stock.""SoldStock"" DESC NULLS LAST ";
                        break;
                    default:
                        sql += @"p.""CreatedAt"" DESC ";
                        break;
                }
            }
            else
                sql += @"ORDER BY p.""CreatedAt"" DESC ";
            return sql;
        }

        public async Task<int> CountListAsync(Guid? categoryId = null, string? search = null)
        {
            using var connection = await ConnectionFactory.CreateOpenConnectionAsync();

            const string sql = @"
                SELECT COUNT(1)
                FROM ""Product"" p
                WHERE p.""DeletedAt"" IS NULL
                  AND (
                      @CategoryId IS NULL
                      OR EXISTS (
                          SELECT 1
                          FROM ""ProductCategory"" pc
                          WHERE pc.""ProductId"" = p.""Id""
                            AND pc.""CategoryId"" = @CategoryId
                            AND pc.""DeletedAt"" IS NULL
                      )
                  )
                  AND (
                      @Search IS NULL
                      OR p.""Name"" ILIKE @SearchPattern
                      OR p.""Slug"" ILIKE @SearchPattern
                      OR p.""Summary"" ILIKE @SearchPattern
                      OR p.""Description"" ILIKE @SearchPattern
                      OR CAST(p.""Code"" AS TEXT) ILIKE @SearchPattern
                      OR EXISTS (
                          SELECT 1
                          FROM ""ProductCategory"" pc
                          INNER JOIN ""Category"" c ON c.""Id"" = pc.""CategoryId""
                          WHERE pc.""ProductId"" = p.""Id""
                            AND pc.""DeletedAt"" IS NULL
                            AND c.""DeletedAt"" IS NULL
                            AND (c.""Name"" ILIKE @SearchPattern OR c.""Slug"" ILIKE @SearchPattern)
                      )
                  )";

            return await connection.ExecuteScalarAsync<int>(sql, new
            {
                CategoryId = categoryId,
                Search = NormalizeSearch(search),
                SearchPattern = BuildSearchPattern(search)
            });
        }

        private static string? NormalizeSearch(string? search)
        {
            return string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        }

        private static string? BuildSearchPattern(string? search)
        {
            var normalizedSearch = NormalizeSearch(search);
            return normalizedSearch == null ? null : $"%{normalizedSearch}%";
        }
    }
}
