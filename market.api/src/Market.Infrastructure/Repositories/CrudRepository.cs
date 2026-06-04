using Dapper;
using Market.Domain.Entities;
using Market.Infrastructure.Data;

namespace Market.Infrastructure.Repositories
{
    public abstract class CrudRepository<T> where T : BaseEntity
    {
        private readonly IDbConnectionFactory _connectionFactory;

        protected CrudRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        protected abstract string TableName { get; }
        protected abstract string SelectColumns { get; }
        protected abstract string InsertColumns { get; }
        protected abstract string InsertValues { get; }
        protected abstract string UpdateAssignments { get; }
        protected IDbConnectionFactory ConnectionFactory => _connectionFactory;

        public async Task<T?> GetByIdAsync(Guid id)
        {
            using var connection = await _connectionFactory.CreateOpenConnectionAsync();

            var sql = $@"
                SELECT {SelectColumns}
                FROM ""{TableName}""
                WHERE ""Id"" = @Id AND ""DeletedAt"" IS NULL";

            return await connection.QueryFirstOrDefaultAsync<T>(sql, new { Id = id });
        }

        public async Task<IEnumerable<T>> GetPagedAsync(int pageIndex, int pageSize)
        {
            using var connection = await _connectionFactory.CreateOpenConnectionAsync();

            var sql = $@"
                SELECT {SelectColumns}
                FROM ""{TableName}""
                WHERE ""DeletedAt"" IS NULL
                ORDER BY ""CreatedAt"" DESC
                LIMIT @PageSize OFFSET @Offset";

            return await connection.QueryAsync<T>(sql, new
            {
                PageSize = NormalizePageSize(pageSize),
                Offset = (NormalizePageIndex(pageIndex) - 1) * NormalizePageSize(pageSize)
            });
        }

        public async Task<int> CountAsync()
        {
            using var connection = await _connectionFactory.CreateOpenConnectionAsync();

            var sql = $@"SELECT COUNT(1) FROM ""{TableName}"" WHERE ""DeletedAt"" IS NULL";

            return await connection.ExecuteScalarAsync<int>(sql);
        }

        public async Task AddAsync(T entity)
        {
            using var connection = await _connectionFactory.CreateOpenConnectionAsync();

            var sql = $@"
                INSERT INTO ""{TableName}""
                ({InsertColumns})
                VALUES
                ({InsertValues})";

            await connection.ExecuteAsync(sql, entity);
        }

        public async Task UpdateAsync(T entity)
        {
            using var connection = await _connectionFactory.CreateOpenConnectionAsync();

            var sql = $@"
                UPDATE ""{TableName}""
                SET {UpdateAssignments}
                WHERE ""Id"" = @Id AND ""DeletedAt"" IS NULL";

            await connection.ExecuteAsync(sql, entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            using var connection = await _connectionFactory.CreateOpenConnectionAsync();

            var sql = $@"
                UPDATE ""{TableName}""
                SET ""DeletedAt"" = @DeletedAt,
                    ""UpdatedAt"" = @DeletedAt
                WHERE ""Id"" = @Id AND ""DeletedAt"" IS NULL";

            await connection.ExecuteAsync(sql, new { Id = id, DeletedAt = DateTime.UtcNow });
        }

        protected static int NormalizePageIndex(int pageIndex) => pageIndex < 1 ? 1 : pageIndex;
        protected static int NormalizePageSize(int pageSize) => pageSize is < 1 or > 100 ? 10 : pageSize;
    }
}
