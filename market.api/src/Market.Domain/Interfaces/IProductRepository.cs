using Market.Domain.Entities;
using Market.Domain.Responses;

namespace Market.Domain.Interfaces
{
    public interface IProductRepository : ICrudRepository<Product>
    {
        Task<bool> CodeExistsAsync(int code, Guid? ignoreId = null);
        Task<IEnumerable<ProductListItemResponse>> GetPagedListAsync(int pageIndex, int pageSize);
    }
}
