using Market.Domain.Entities;

namespace Market.Domain.Interfaces
{
    public interface IErrorLogRepository
    {
        Task AddAsync(ErrorLog errorLog);
    }
}
