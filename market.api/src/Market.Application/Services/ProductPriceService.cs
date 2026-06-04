using Market.Application.DTOs;
using Market.Application.Interfaces;
using Market.Domain.Entities;
using Market.Domain.Exceptions;
using Market.Domain.Interfaces;

namespace Market.Application.Services
{
    public class ProductPriceService : CrudService<ProductPrice, ProductPriceDto, ProductPriceCreateDto, ProductPriceUpdateDto>, IProductPriceService
    {
        private readonly IProductPriceRepository _repository;

        public ProductPriceService(IProductPriceRepository repository) : base(repository, "ProductPrice")
        {
            _repository = repository;
        }

        protected override async Task ValidateCreateAsync(ProductPriceCreateDto dto)
        {
            ValidateInterval(dto.ValidFrom, dto.ValidTo);

            if (await _repository.HasOverlappingIntervalAsync(dto.ProductId, dto.ValidFrom, dto.ValidTo))
                throw new BusinessException("ProductPrice interval already exists for this product");
        }

        protected override async Task ValidateUpdateAsync(ProductPriceUpdateDto dto)
        {
            ValidateInterval(dto.ValidFrom, dto.ValidTo);

            if (await _repository.HasOverlappingIntervalAsync(dto.ProductId, dto.ValidFrom, dto.ValidTo, dto.Id))
                throw new BusinessException("ProductPrice interval already exists for this product");
        }

        protected override Guid GetId(ProductPriceUpdateDto dto) => dto.Id;

        protected override ProductPrice ToNewEntity(ProductPriceCreateDto dto) => new()
        {
            ProductId = dto.ProductId,
            OriginalPrice = dto.OriginalPrice,
            SalePrice = dto.SalePrice,
            Currency = dto.Currency,
            ValidFrom = dto.ValidFrom,
            ValidTo = dto.ValidTo
        };

        protected override void ApplyUpdate(ProductPriceUpdateDto dto, ProductPrice entity)
        {
            entity.ProductId = dto.ProductId;
            entity.OriginalPrice = dto.OriginalPrice;
            entity.SalePrice = dto.SalePrice;
            entity.Currency = dto.Currency;
            entity.ValidFrom = dto.ValidFrom;
            entity.ValidTo = dto.ValidTo;
            entity.Status = dto.Status;
        }

        protected override ProductPriceDto ToDto(ProductPrice entity) => new()
        {
            Id = entity.Id,
            ProductId = entity.ProductId,
            OriginalPrice = entity.OriginalPrice,
            SalePrice = entity.SalePrice,
            Currency = entity.Currency,
            ValidFrom = entity.ValidFrom,
            ValidTo = entity.ValidTo,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Status = entity.Status
        };

        private static void ValidateInterval(DateTime validFrom, DateTime? validTo)
        {
            if (validTo.HasValue && validTo.Value < validFrom)
                throw new BusinessException("ValidTo must be greater than or equal to ValidFrom");
        }
    }
}
