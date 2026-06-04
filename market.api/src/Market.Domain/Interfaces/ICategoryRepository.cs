using Market.Domain.Entities;

namespace Market.Domain.Interfaces
{
    public interface ICategoryRepository : ICrudRepository<Category>
    {
        Task<bool> SlugExistsAsync(string slug, Guid? ignoreId = null);
    }
}
