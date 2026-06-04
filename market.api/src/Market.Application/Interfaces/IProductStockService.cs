using Market.Application.DTOs;

namespace Market.Application.Interfaces
{
    public interface IProductStockService : ICrudService<ProductStockDto, ProductStockCreateDto, ProductStockUpdateDto>
    {
    }
}
