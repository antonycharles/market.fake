using System.Data.Common;

namespace Market.Infrastructure.Data
{
    public interface IDbConnectionFactory
    {
        Task<DbConnection> CreateOpenConnectionAsync();
    }
}
