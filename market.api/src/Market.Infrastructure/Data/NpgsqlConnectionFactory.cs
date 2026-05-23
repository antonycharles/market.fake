using System.Data.Common;
using Market.Domain.Settings;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Market.Infrastructure.Data
{
    public sealed class NpgsqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public NpgsqlConnectionFactory(IOptions<ProjectSettings> options)
        {
            _connectionString = options.Value.ConnectionString;
        }

        public async Task<DbConnection> CreateOpenConnectionAsync()
        {
            var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}
