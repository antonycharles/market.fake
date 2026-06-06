using Market.Application.DTOs;
using Market.Application.Interfaces;
using Market.Domain.Entities;
using Market.Domain.Interfaces;

namespace Market.Application.Services
{
    public class ProductPhotoService : CrudService<ProductPhoto, ProductPhotoDto, ProductPhotoCreateDto, ProductPhotoUpdateDto>, IProductPhotoService
    {
        private readonly IProductPhotoRepository _repository;

        public ProductPhotoService(IProductPhotoRepository repository) : base(repository, "ProductPhoto")
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ProductPhotoDto>> GetByProductIdAsync(Guid productId)
        {
            var photos = await _repository.GetByProductIdAsync(productId);

            return photos.Select(ToDto);
        }

        protected override Guid GetId(ProductPhotoUpdateDto dto) => dto.Id;

        protected override ProductPhoto ToNewEntity(ProductPhotoCreateDto dto) => new()
        {
            ProductId = dto.ProductId,
            FileId = dto.FileId,
            Url = dto.Url,
            Description = dto.Description,
            Order = dto.Order,
            Type = dto.Type
        };

        protected override void ApplyUpdate(ProductPhotoUpdateDto dto, ProductPhoto entity)
        {
            entity.ProductId = dto.ProductId;
            entity.FileId = dto.FileId;
            entity.Url = dto.Url;
            entity.Description = dto.Description;
            entity.Order = dto.Order;
            entity.Type = dto.Type;
            entity.Status = dto.Status;
        }

        protected override ProductPhotoDto ToDto(ProductPhoto entity) => new()
        {
            Id = entity.Id,
            ProductId = entity.ProductId,
            FileId = entity.FileId,
            Url = entity.Url,
            Description = entity.Description,
            Order = entity.Order,
            Type = entity.Type,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Status = entity.Status
        };
    }
}
