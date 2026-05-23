using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Market.Application.Interfaces;
using Market.Application.DTOs;
using Market.Domain.Interfaces;
using Market.Domain.Entities;
using Market.Domain.Exceptions;
using Market.Domain.Interfaces.Externals;
using Market.Domain.Responses;

namespace Market.Application.Services
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IUserRepository _userRepository;

        public MemberService(IMemberRepository memberRepository, IUserRepository userRepository)
        {
            _memberRepository = memberRepository;
            _userRepository = userRepository;
        }

        public async Task<MemberDto?> GetByIdAsync(Guid id)
        {
            var member = await _memberRepository.GetByIdAsync(id);
            if (member == null) throw new BusinessException("Member not found");
            return MapToDto(member);
        }

        public async Task<IEnumerable<MemberDto>> GetByProjectIdAsync(Guid projectId)
        {
            var members = await _memberRepository.GetByProjectIdAsync(projectId);
            var users = await _userRepository.GetUsersByIdsAsync(members.Select(m => m.UserId).ToList());
            return MapToDtoList(members, users);
        }

        public async Task<MemberDto> AddAsync(MemberCreateDto dto)
        {
            var member = MapToNewMember(dto);
            
            await _memberRepository.AddAsync(member);
            return MapToDto(member);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _memberRepository.DeleteAsync(id);
        }

        private static Member MapToNewMember(MemberCreateDto dto)
        {
            return new Member
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                ProjectId = dto.ProjectId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        private static MemberDto MapToDto(Member member, UserResponse? user = null)
        {
            return new MemberDto
            {
                Id = member.Id,
                UserId = member.UserId,
                UserName = user?.Name,
                ProjectId = member.ProjectId,
                CreatedAt = member.CreatedAt,
                UpdatedAt = member.UpdatedAt,
                Status = member.Status
            };
        }

        private static IEnumerable<MemberDto> MapToDtoList(IEnumerable<Member> members, List<UserResponse>? users = null)
        {
            foreach (var member in members)
            {
                yield return MapToDto(member, users?.FirstOrDefault(u => u.Id == member.UserId));
            }
        }
    }
}