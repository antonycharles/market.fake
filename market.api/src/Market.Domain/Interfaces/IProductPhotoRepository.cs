using Market.Domain.Entities;

namespace Market.Domain.Interfaces
{
    public interface IProductPhotoRepository : ICrudRepository<ProductPhoto>
    {
        Task<IEnumerable<ProductPhoto>> GetByProductIdAsync(Guid productId);
    }
}
