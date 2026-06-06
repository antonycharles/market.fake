using Market.Application.DTOs;
using Market.Application.Interfaces;
using Market.Domain.Entities;
using Market.Domain.Exceptions;
using Market.Domain.Interfaces;

namespace Market.Application.Services
{
    public class ProductStockService : CrudService<ProductStock, ProductStockDto, ProductStockCreateDto, ProductStockUpdateDto>, IProductStockService
    {
        private readonly IProductStockRepository _repository;

        public ProductStockService(IProductStockRepository repository) : base(repository, "ProductStock")
        {
            _repository = repository;
        }

        public async Task<ProductStockDto?> GetByProductIdAsync(Guid productId)
        {
            var stock = await _repository.GetByProductIdAsync(productId);

            return stock is null ? null : ToDto(stock);
        }

        protected override async Task ValidateCreateAsync(ProductStockCreateDto dto)
        {
            if (await _repository.ProductIdExistsAsync(dto.ProductId))
                throw new BusinessException("ProductStock already exists for this product");
        }

        protected override async Task ValidateUpdateAsync(ProductStockUpdateDto dto)
        {
            if (await _repository.ProductIdExistsAsync(dto.ProductId, dto.Id))
                throw new BusinessException("ProductStock already exists for this product");
        }

        protected override Guid GetId(ProductStockUpdateDto dto) => dto.Id;

        protected override ProductStock ToNewEntity(ProductStockCreateDto dto) => new()
        {
            ProductId = dto.ProductId,
            AvailableStock = dto.AvailableStock,
            ReservedStock = dto.ReservedStock,
            SoldStock = dto.SoldStock
        };

        protected override void ApplyUpdate(ProductStockUpdateDto dto, ProductStock entity)
        {
            entity.ProductId = dto.ProductId;
            entity.AvailableStock = dto.AvailableStock;
            entity.ReservedStock = dto.ReservedStock;
            entity.SoldStock = dto.SoldStock;
            entity.Status = dto.Status;
        }

        protected override ProductStockDto ToDto(ProductStock entity) => new()
        {
            Id = entity.Id,
            ProductId = entity.ProductId,
            AvailableStock = entity.AvailableStock,
            ReservedStock = entity.ReservedStock,
            SoldStock = entity.SoldStock,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Status = entity.Status
        };
    }
}
