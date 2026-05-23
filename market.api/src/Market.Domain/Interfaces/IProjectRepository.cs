using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Market.Domain.Interfaces
{
    public interface IProjectRepository
    {
        Task<Entities.Project> GetByIdAsync(Guid id);
        Task<IEnumerable<Entities.Project>> GetAllAsync();
        Task AddAsync(Entities.Project Project);
        Task UpdateAsync(Entities.Project Project);
        Task DeleteAsync(Guid id);
    }
}