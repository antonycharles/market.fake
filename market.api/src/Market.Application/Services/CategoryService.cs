using Market.Application.DTOs;
using Market.Application.Interfaces;
using Market.Domain.Entities;
using Market.Domain.Exceptions;
using Market.Domain.Interfaces;

namespace Market.Application.Services
{
    public class CategoryService : CrudService<Category, CategoryDto, CategoryCreateDto, CategoryUpdateDto>, ICategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository repository) : base(repository, "Category")
        {
            _repository = repository;
        }

        protected override async Task ValidateCreateAsync(CategoryCreateDto dto)
        {
            if (await _repository.SlugExistsAsync(dto.Slug))
                throw new BusinessException("Category slug already exists");
        }

        protected override async Task ValidateUpdateAsync(CategoryUpdateDto dto)
        {
            if (await _repository.SlugExistsAsync(dto.Slug, dto.Id))
                throw new BusinessException("Category slug already exists");
        }

        protected override Guid GetId(CategoryUpdateDto dto) => dto.Id;

        protected override Category ToNewEntity(CategoryCreateDto dto) => new()
        {
            Name = dto.Name,
            Slug = dto.Slug,
            Description = dto.Description
        };

        protected override void ApplyUpdate(CategoryUpdateDto dto, Category entity)
        {
            entity.Name = dto.Name;
            entity.Slug = dto.Slug;
            entity.Description = dto.Description;
            entity.Status = dto.Status;
        }

        protected override CategoryDto ToDto(Category entity) => new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Slug = entity.Slug,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Status = entity.Status
        };
    }
}
