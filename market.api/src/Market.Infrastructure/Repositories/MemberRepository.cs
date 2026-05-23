using Dapper;
using Market.Domain.Entities;
using Market.Domain.Enums;
using Market.Domain.Interfaces;
using Market.Infrastructure.Data;

namespace Market.Infrastructure.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public MemberRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Member?> GetByIdAsync(Guid id)
        {
            using var connection = await _connectionFactory.CreateOpenConnectionAsync();

            const string sql = @"
                SELECT ""Id"", ""UserId"", ""ProjectId"", ""CreatedAt"", ""UpdatedAt"", ""Status""
                FROM ""Member""
                WHERE ""Id"" = @Id AND ""DeletedAt"" IS NULL";

            return await connection.QueryFirstOrDefaultAsync<Member>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Member>> GetByProjectIdAsync(Guid projectId)
        {
            using var connection = await _connectionFactory.CreateOpenConnectionAsync();

            const string sql = @"
                SELECT ""Id"", ""UserId"", ""ProjectId"", ""CreatedAt"", ""UpdatedAt"", ""Status""
                FROM ""Member""
                WHERE ""ProjectId"" = @ProjectId AND ""DeletedAt"" IS NULL";

            return await connection.QueryAsync<Member>(sql, new { ProjectId = projectId });
        }

        public async Task AddAsync(Member member)
        {
            using var connection = await _connectionFactory.CreateOpenConnectionAsync();

            const string sql = @"
                INSERT INTO ""Member"" 
                (""Id"", ""UserId"", ""ProjectId"", ""CreatedAt"", ""UpdatedAt"", ""Status"")
                VALUES
                (@Id, @UserId, @ProjectId, @CreatedAt, @UpdatedAt, @Status)";

            await connection.ExecuteAsync(sql, new
            {
                member.Id,
                member.UserId,
                member.ProjectId,
                member.CreatedAt,
                member.UpdatedAt,
                Status = (int)StatusEnum.Active
            });
        }

        public async Task DeleteAsync(Guid id)
        {
            using var connection = await _connectionFactory.CreateOpenConnectionAsync();

            const string sql = @"
                UPDATE ""Member""
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
