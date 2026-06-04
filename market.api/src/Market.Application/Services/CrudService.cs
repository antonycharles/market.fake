using Market.Application.DTOs;
using Market.Application.Interfaces;
using Market.Domain.Entities;
using Market.Domain.Enums;
using Market.Domain.Exceptions;
using Market.Domain.Interfaces;
using Market.Domain.Responses;

namespace Market.Application.Services
{
    public abstract class CrudService<TEntity, TDto, TCreateDto, TUpdateDto> : ICrudService<TDto, TCreateDto, TUpdateDto>
        where TEntity : BaseEntity
    {
        private readonly ICrudRepository<TEntity> _repository;
        private readonly string _entityName;

        protected CrudService(ICrudRepository<TEntity> repository, string entityName)
        {
            _repository = repository;
            _entityName = entityName;
        }

        public async Task<TDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);

            if (entity == null)
                throw new BusinessException($"{_entityName} not found");

            return ToDto(entity);
        }

        public async Task<PaginatedResponse<TDto>> GetPagedAsync(PaginationRequestDto request)
        {
            var pageIndex = NormalizePageIndex(request.PageIndex);
            var pageSize = NormalizePageSize(request.PageSize);
            var items = await _repository.GetPagedAsync(pageIndex, pageSize);
            var total = await _repository.CountAsync();

            return new PaginatedResponse<TDto>(items.Select(ToDto).ToList(), total, pageIndex, pageSize, request);
        }

        public async Task<TDto> AddAsync(TCreateDto dto)
        {
            await ValidateCreateAsync(dto);

            var entity = ToNewEntity(dto);
            entity.Id = Guid.NewGuid();
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.Status = StatusEnum.Active;

            await _repository.AddAsync(entity);

            return ToDto(entity);
        }

        public async Task<TDto> UpdateAsync(TUpdateDto dto)
        {
            var entity = await _repository.GetByIdAsync(GetId(dto));

            if (entity == null)
                throw new BusinessException($"{_entityName} not found");

            await ValidateUpdateAsync(dto);

            ApplyUpdate(dto, entity);
            entity.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(entity);

            return ToDto(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);

            if (entity == null)
                throw new BusinessException($"{_entityName} not found");

            await _repository.DeleteAsync(id);
        }

        protected virtual Task ValidateCreateAsync(TCreateDto dto) => Task.CompletedTask;
        protected virtual Task ValidateUpdateAsync(TUpdateDto dto) => Task.CompletedTask;
        protected abstract Guid GetId(TUpdateDto dto);
        protected abstract TEntity ToNewEntity(TCreateDto dto);
        protected abstract void ApplyUpdate(TUpdateDto dto, TEntity entity);
        protected abstract TDto ToDto(TEntity entity);

        protected static int NormalizePageIndex(int pageIndex) => pageIndex < 1 ? 1 : pageIndex;
        protected static int NormalizePageSize(int pageSize) => pageSize is < 1 or > 100 ? 10 : pageSize;
    }
}
