using Dapper;
using Market.Domain.Entities;
using Market.Domain.Interfaces;
using Market.Infrastructure.Data;

namespace Market.Infrastructure.Repositories
{
    public class CategoryRepository : CrudRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        protected override string TableName => "Category";
        protected override string SelectColumns => @"""Id"", ""Name"", ""Slug"", ""Description"", ""CreatedAt"", ""UpdatedAt"", ""DeletedAt"", ""Status""";
        protected override string InsertColumns => @"""Id"", ""Name"", ""Slug"", ""Description"", ""CreatedAt"", ""UpdatedAt"", ""Status""";
        protected override string InsertValues => @"@Id, @Name, @Slug, @Description, @CreatedAt, @UpdatedAt, @Status";
        protected override string UpdateAssignments => @"""Name"" = @Name, ""Slug"" = @Slug, ""Description"" = @Description, ""Status"" = @Status, ""UpdatedAt"" = @UpdatedAt";

        public async Task<Category?> GetBySlugAsync(string slug)
        {
            using var connection = await ConnectionFactory.CreateOpenConnectionAsync();

            var sql = $@"
                SELECT {SelectColumns}
                FROM ""{TableName}""
                WHERE ""Slug"" = @Slug AND ""DeletedAt"" IS NULL";

            return await connection.QueryFirstOrDefaultAsync<Category>(sql, new { Slug = slug });
        }

        public async Task<bool> SlugExistsAsync(string slug, Guid? ignoreId = null)
        {
            using var connection = await ConnectionFactory.CreateOpenConnectionAsync();

            const string sql = @"
                SELECT COUNT(1)
                FROM ""Category""
                WHERE ""DeletedAt"" IS NULL
                  AND LOWER(""Slug"") = LOWER(@Slug)
                  AND (@IgnoreId IS NULL OR ""Id"" <> @IgnoreId)";

            return await connection.ExecuteScalarAsync<int>(sql, new { Slug = slug, IgnoreId = ignoreId }) > 0;
        }
    }
}
