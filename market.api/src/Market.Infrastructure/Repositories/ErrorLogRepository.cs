using Dapper;
using Market.Domain.Entities;
using Market.Domain.Interfaces;
using Market.Infrastructure.Data;

namespace Market.Infrastructure.Repositories
{
    public class ErrorLogRepository : IErrorLogRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ErrorLogRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task AddAsync(ErrorLog errorLog)
        {
            using var connection = await _connectionFactory.CreateOpenConnectionAsync();

            const string sql = @"
                INSERT INTO ""ErrorLog""
                (""Id"", ""Source"", ""Message"", ""StackTrace"", ""RequestPath"", ""HttpMethod"", ""CreatedAt"", ""UpdatedAt"", ""Status"")
                VALUES
                (@Id, @Source, @Message, @StackTrace, @RequestPath, @HttpMethod, @CreatedAt, @UpdatedAt, @Status)";

            await connection.ExecuteAsync(sql, errorLog);
        }
    }
}
