using Market.Application.DTOs;
using Market.Domain.Responses;

namespace Market.Application.Interfaces
{
    public interface IProductService : ICrudService<ProductDto, ProductCreateDto, ProductUpdateDto>
    {
        Task<PaginatedResponse<ProductListItemDto>> GetProductListAsync(PaginationRequestDto request);
    }
}
