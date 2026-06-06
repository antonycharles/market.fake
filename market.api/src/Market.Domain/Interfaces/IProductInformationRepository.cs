using Market.Domain.Entities;

namespace Market.Domain.Interfaces
{
    public interface IProductInformationRepository : ICrudRepository<ProductInformation>
    {
        Task<IEnumerable<ProductInformation>> GetByProductIdAsync(Guid productId);
    }
}
