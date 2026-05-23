using Dapper;
using Market.Domain.Interfaces;
using Market.Infrastructure.Data;

namespace Market.Infrastructure.Repositories
{
    public class ProjectRepository: IProjectRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProjectRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Domain.Entities.Project?> GetByIdAsync(Guid id)
        {
            using var connection = await _connectionFactory.CreateOpenConnectionAsync();

            const string sql = @"
                SELECT ""Id"", ""Name"", ""Description"", ""UserCreatedId"", ""CreatedAt"", ""UpdatedAt"", ""Status""
                FROM ""Project""
                WHERE ""Id"" = @Id AND ""DeletedAt"" IS NULL";

            return await connection.QueryFirstOrDefaultAsync<Domain.Entities.Project>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Domain.Entities.Project>> GetAllAsync()
        {
            using var connection = await _connectionFactory.CreateOpenConnectionAsync();

            const string sql = @"
                SELECT ""Id"", ""Name"", ""Description"", ""UserCreatedId"", ""CreatedAt"", ""UpdatedAt"", ""Status""
                FROM ""Project""
                WHERE ""DeletedAt"" IS NULL";

            return await connection.QueryAsync<Domain.Entities.Project>(sql);
        }

        public async Task AddAsync(Domain.Entities.Project Project)
        {
            using var connection = await _connectionFactory.CreateOpenConnectionAsync();

            const string sql = @"
                INSERT INTO ""Project"" 
                (""Id"", ""Name"", ""Description"", ""UserCreatedId"", ""CreatedAt"", ""UpdatedAt"", ""Status"")
                VALUES
                (@Id, @Name, @Description, @UserCreatedId, @CreatedAt, @UpdatedAt, @Status)";

            await connection.ExecuteAsync(sql, new
            {
                Project.Id,
                Project.Name,
                Project.Description,
                Project.UserCreatedId,
                Project.CreatedAt,
                Project.UpdatedAt,
                Status = (int)Project.Status
            });
        }

        public async Task UpdateAsync(Domain.Entities.Project Project)
        {
            using var connection = await _connectionFactory.CreateOpenConnectionAsync();

            const string sql = @"
                UPDATE ""Project""
                SET ""Name"" = @Name,
                    ""Description"" = @Description,
                    ""Status"" = @Status,
                    ""UpdatedAt"" = @UpdatedAt
                WHERE ""Id"" = @Id";

            await connection.ExecuteAsync(sql, new
            {
                Project.Id,
                Project.Name,
                Project.Description,
                Project.UpdatedAt,
                Status = (int)Project.Status
            });
        }

        public async Task DeleteAsync(Guid id)
        {
            using var connection = await _connectionFactory.CreateOpenConnectionAsync();

            const string sql = @"
                UPDATE ""Project""
                SET ""DeletedAt"" = @DeletedAt
                WHERE ""Id"" = @Id";

            await connection.ExecuteAsync(sql, new
            {
                Id = id,
                DeletedAt = DateTime.UtcNow
            });
        }
    }
}
