using Market.Application.DTOs;
using Market.Domain.Responses;

namespace Market.Application.Interfaces
{
    public interface ICrudService<TDto, TCreateDto, TUpdateDto>
    {
        Task<TDto?> GetByIdAsync(Guid id);
        Task<PaginatedResponse<TDto>> GetPagedAsync(PaginationRequestDto request);
        Task<TDto> AddAsync(TCreateDto dto);
        Task<TDto> UpdateAsync(TUpdateDto dto);
        Task DeleteAsync(Guid id);
    }
}
