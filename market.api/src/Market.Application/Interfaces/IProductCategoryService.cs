using Market.Application.DTOs;

namespace Market.Application.Interfaces
{
    public interface IProductCategoryService : ICrudService<ProductCategoryDto, ProductCategoryCreateDto, ProductCategoryUpdateDto>
    {
        Task<IEnumerable<CategoryDto>> GetCategoriesByProductIdAsync(Guid productId);
    }
}
