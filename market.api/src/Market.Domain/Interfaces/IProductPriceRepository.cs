using Market.Domain.Entities;

namespace Market.Domain.Interfaces
{
    public interface IProductPriceRepository : ICrudRepository<ProductPrice>
    {
        Task<bool> HasOverlappingIntervalAsync(Guid productId, DateTime validFrom, DateTime? validTo, Guid? ignoreId = null);
        Task<ProductPrice?> GetCurrentByProductIdAsync(Guid productId, DateTime now);
    }
}
