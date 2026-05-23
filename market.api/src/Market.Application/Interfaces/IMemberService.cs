using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Market.Application.DTOs;

namespace Market.Application.Interfaces
{
    public interface IMemberService
    {
        Task<MemberDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<MemberDto>> GetByProjectIdAsync(Guid projectId);
        Task<MemberDto> AddAsync(MemberCreateDto dto);
        Task DeleteAsync(Guid id);
    }
}