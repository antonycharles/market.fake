using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Market.Domain.Entities;

namespace Market.Domain.Interfaces
{
    public interface IMemberRepository
    {
        Task<Member> GetByIdAsync(Guid id);
        Task<IEnumerable<Member>> GetByProjectIdAsync(Guid projectId);
        Task AddAsync(Member member);
        Task DeleteAsync(Guid id);
    }
}