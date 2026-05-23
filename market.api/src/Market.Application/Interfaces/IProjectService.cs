using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Market.Application.DTOs;

namespace Market.Application.Interfaces
{
    public interface IProjectService
    {
        Task<ProjectDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<ProjectDto>> GetAllAsync();
        Task<ProjectDto> AddAsync(ProjectCreateDto dto);
        Task UpdateAsync(ProjectUpdateDto dto);
        Task DeleteAsync(Guid id);
    }
}