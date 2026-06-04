using Market.Application.DTOs;

namespace Market.Application.Interfaces
{
    public interface ICategoryService : ICrudService<CategoryDto, CategoryCreateDto, CategoryUpdateDto>
    {
    }
}
