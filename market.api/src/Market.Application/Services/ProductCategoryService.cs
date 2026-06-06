using Market.Application.DTOs;
using Market.Application.Interfaces;
using Market.Domain.Entities;
using Market.Domain.Interfaces;

namespace Market.Application.Services
{
    public class ProductCategoryService : CrudService<ProductCategory, ProductCategoryDto, ProductCategoryCreateDto, ProductCategoryUpdateDto>, IProductCategoryService
    {
        private readonly IProductCategoryRepository _repository;

        public ProductCategoryService(IProductCategoryRepository repository) : base(repository, "ProductCategory")
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CategoryDto>> GetCategoriesByProductIdAsync(Guid productId)
        {
            var categories = await _repository.GetCategoriesByProductIdAsync(productId);

            return categories.Select(ToCategoryDto);
        }

        protected override Guid GetId(ProductCategoryUpdateDto dto) => dto.Id;

        protected override ProductCategory ToNewEntity(ProductCategoryCreateDto dto) => new()
        {
            ProductId = dto.ProductId,
            CategoryId = dto.CategoryId,
            Order = dto.Order
        };

        protected override void ApplyUpdate(ProductCategoryUpdateDto dto, ProductCategory entity)
        {
            entity.ProductId = dto.ProductId;
            entity.CategoryId = dto.CategoryId;
            entity.Order = dto.Order;
            entity.Status = dto.Status;
        }

        protected override ProductCategoryDto ToDto(ProductCategory entity) => new()
        {
            Id = entity.Id,
            ProductId = entity.ProductId,
            CategoryId = entity.CategoryId,
            Order = entity.Order,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Status = entity.Status
        };

        private static CategoryDto ToCategoryDto(Category entity) => new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Slug = entity.Slug,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Status = entity.Status
        };
    }
}
