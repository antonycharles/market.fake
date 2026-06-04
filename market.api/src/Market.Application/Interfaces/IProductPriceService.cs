using Market.Application.DTOs;

namespace Market.Application.Interfaces
{
    public interface IProductPriceService : ICrudService<ProductPriceDto, ProductPriceCreateDto, ProductPriceUpdateDto>
    {
    }
}
