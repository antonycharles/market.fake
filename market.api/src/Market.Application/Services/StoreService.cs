using Market.Application.DTOs;
using Market.Application.Interfaces;
using Market.Domain.Entities;
using Market.Domain.Interfaces;

namespace Market.Application.Services
{
    public class StoreService : CrudService<Store, StoreDto, StoreCreateDto, StoreUpdateDto>, IStoreService
    {
        public StoreService(IStoreRepository repository) : base(repository, "Store")
        {
        }

        protected override Guid GetId(StoreUpdateDto dto) => dto.Id;

        protected override Store ToNewEntity(StoreCreateDto dto) => new()
        {
            Name = dto.Name,
            Description = dto.Description,
            UserCreatedId = dto.UserCreatedId
        };

        protected override void ApplyUpdate(StoreUpdateDto dto, Store entity)
        {
            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.UserCreatedId = dto.UserCreatedId;
            entity.Status = dto.Status;
        }

        protected override StoreDto ToDto(Store entity) => new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            UserCreatedId = entity.UserCreatedId,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Status = entity.Status
        };
    }
}
