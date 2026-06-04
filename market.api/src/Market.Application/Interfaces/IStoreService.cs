using Market.Application.DTOs;

namespace Market.Application.Interfaces
{
    public interface IStoreService : ICrudService<StoreDto, StoreCreateDto, StoreUpdateDto>
    {
    }
}
