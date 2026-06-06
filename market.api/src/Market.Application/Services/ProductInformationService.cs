using Market.Application.DTOs;
using Market.Application.Interfaces;
using Market.Domain.Entities;
using Market.Domain.Interfaces;

namespace Market.Application.Services
{
    public class ProductInformationService : CrudService<ProductInformation, ProductInformationDto, ProductInformationCreateDto, ProductInformationUpdateDto>, IProductInformationService
    {
        private readonly IProductInformationRepository _repository;

        public ProductInformationService(IProductInformationRepository repository) : base(repository, "ProductInformation")
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ProductInformationDto>> GetByProductIdAsync(Guid productId)
        {
            var informations = await _repository.GetByProductIdAsync(productId);

            return informations.Select(ToDto);
        }

        protected override Guid GetId(ProductInformationUpdateDto dto) => dto.Id;

        protected override ProductInformation ToNewEntity(ProductInformationCreateDto dto) => new()
        {
            ProductId = dto.ProductId,
            Type = dto.Type,
            Label = dto.Label,
            Value = dto.Value,
            Order = dto.Order
        };

        protected override void ApplyUpdate(ProductInformationUpdateDto dto, ProductInformation entity)
        {
            entity.ProductId = dto.ProductId;
            entity.Type = dto.Type;
            entity.Label = dto.Label;
            entity.Value = dto.Value;
            entity.Order = dto.Order;
            entity.Status = dto.Status;
        }

        protected override ProductInformationDto ToDto(ProductInformation entity) => new()
        {
            Id = entity.Id,
            ProductId = entity.ProductId,
            Type = entity.Type,
            Label = entity.Label,
            Value = entity.Value,
            Order = entity.Order,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Status = entity.Status
        };
    }
}
