using Market.Domain.Entities;

namespace Market.Domain.Interfaces
{
    public interface IProductStockRepository : ICrudRepository<ProductStock>
    {
        Task<bool> ProductIdExistsAsync(Guid productId, Guid? ignoreId = null);
        Task<ProductStock?> GetByProductIdAsync(Guid productId);
    }
}
