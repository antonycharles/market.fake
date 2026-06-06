using Market.Domain.Entities;
using Market.Domain.Enums;
using Market.Domain.Responses;

namespace Market.Domain.Interfaces
{
    public interface IProductRepository : ICrudRepository<Product>
    {
        Task<bool> CodeExistsAsync(int code, Guid? ignoreId = null);
        Task<Product?> GetByCodeAsync(int code);
        Task<IEnumerable<ProductListItemResponse>> GetPagedListAsync(int pageIndex, int pageSize, Guid? categoryId = null, string? search = null, ProductOrderEnum? order = null);
        Task<int> CountListAsync(Guid? categoryId = null, string? search = null);
    }
}
