using Market.Domain.Entities;

namespace Market.Domain.Interfaces
{
    public interface ICrudRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetPagedAsync(int pageIndex, int pageSize);
        Task<int> CountAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id);
    }
}
