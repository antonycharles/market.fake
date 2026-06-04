using Market.Application.DTOs;

namespace Market.Application.Interfaces
{
    public interface IProductInformationService : ICrudService<ProductInformationDto, ProductInformationCreateDto, ProductInformationUpdateDto>
    {
    }
}
