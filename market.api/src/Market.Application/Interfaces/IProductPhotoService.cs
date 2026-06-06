using Market.Application.DTOs;

namespace Market.Application.Interfaces
{
    public interface IProductPhotoService : ICrudService<ProductPhotoDto, ProductPhotoCreateDto, ProductPhotoUpdateDto>
    {
        Task<IEnumerable<ProductPhotoDto>> GetByProductIdAsync(Guid productId);
    }
}
