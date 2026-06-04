using Market.Application.DTOs;
using Market.Application.Interfaces;
using Market.Domain.Entities;
using Market.Domain.Enums;
using Market.Domain.Exceptions;
using Market.Domain.Interfaces;
using Market.Domain.Responses;

namespace Market.Application.Services
{
    public class ProductService : CrudService<Product, ProductDto, ProductCreateDto, ProductUpdateDto>, IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository) : base(repository, "Product")
        {
            _repository = repository;
        }

        public async Task<PaginatedResponse<ProductListItemDto>> GetProductListAsync(PaginationRequestDto request)
        {
            var pageIndex = NormalizePageIndex(request.PageIndex);
            var pageSize = NormalizePageSize(request.PageSize);
            var items = await _repository.GetPagedListAsync(pageIndex, pageSize);
            var total = await _repository.CountAsync();

            return new PaginatedResponse<ProductListItemDto>(
                items.Select(ToListItemDto).ToList(),
                total,
                pageIndex,
                pageSize,
                request);
        }

        protected override async Task ValidateCreateAsync(ProductCreateDto dto)
        {
            if (await _repository.CodeExistsAsync(dto.Code))
                throw new BusinessException("Product code already exists");
        }

        protected override async Task ValidateUpdateAsync(ProductUpdateDto dto)
        {
            if (await _repository.CodeExistsAsync(dto.Code, dto.Id))
                throw new BusinessException("Product code already exists");
        }

        protected override Guid GetId(ProductUpdateDto dto) => dto.Id;

        protected override Product ToNewEntity(ProductCreateDto dto) => new()
        {
            StoreId = dto.StoreId,
            Name = dto.Name,
            Code = dto.Code,
            Slug = dto.Slug,
            Summary = dto.Summary,
            Description = dto.Description
        };

        protected override void ApplyUpdate(ProductUpdateDto dto, Product entity)
        {
            entity.StoreId = dto.StoreId;
            entity.Name = dto.Name;
            entity.Code = dto.Code;
            entity.Slug = dto.Slug;
            entity.Summary = dto.Summary;
            entity.Description = dto.Description;
            entity.Status = dto.Status;
        }

        protected override ProductDto ToDto(Product entity) => new()
        {
            Id = entity.Id,
            StoreId = entity.StoreId,
            Name = entity.Name,
            Code = entity.Code,
            Slug = entity.Slug,
            Summary = entity.Summary,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Status = entity.Status
        };

        private static ProductListItemDto ToListItemDto(ProductListItemResponse item) => new()
        {
            Id = item.Id,
            Name = item.Name,
            Code = item.Code,
            Slug = item.Slug,
            Summary = item.Summary,
            ProductPrice = item.ProductPriceId.HasValue
                ? new ProductPriceDto
                {
                    Id = item.ProductPriceId.Value,
                    OriginalPrice = item.OriginalPrice.GetValueOrDefault(),
                    SalePrice = item.SalePrice.GetValueOrDefault(),
                    Currency = item.Currency,
                    ValidFrom = item.ValidFrom.GetValueOrDefault(),
                    ValidTo = item.ValidTo
                }
                : null,
            ProductPhoto = item.ProductPhotoId.HasValue
                ? new ProductPhotoDto
                {
                    Id = item.ProductPhotoId.Value,
                    FileId = item.PhotoFileId,
                    Url = item.PhotoUrl,
                    Description = item.PhotoDescription,
                    Type = ProductPhotoEnum.Principal
                }
                : null
        };
    }
}
