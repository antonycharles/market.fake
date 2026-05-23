using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Market.Application.Interfaces;
using Market.Application.DTOs;
using Market.Domain.Interfaces;
using Market.Domain.Entities;
using Market.Domain.Exceptions;
using Market.Domain.Enums;
using Messaging.Abstractions;
using Messaging.Contracts.Events;

namespace Market.Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _ProjectRepository;
        private readonly IMemberService _memberService;
        private readonly IEventBus _eventBus;

        public ProjectService(IProjectRepository ProjectRepository, IMemberService memberService, IEventBus eventBus)
        {
            _ProjectRepository = ProjectRepository;
            _memberService = memberService;
            _eventBus = eventBus;
        }

        public async Task<ProjectDto?> GetByIdAsync(Guid id)
        {
            var Project = await _ProjectRepository.GetByIdAsync(id);

            if (Project == null) throw new BusinessException("Project not found");

            var result = MapToDto(Project);

            result.Members = await _memberService.GetByProjectIdAsync(id);

            return result;
        }

        public async Task<IEnumerable<ProjectDto>> GetAllAsync()
        {
            var families = await _ProjectRepository.GetAllAsync();

            return families.Select(MapToDto);
        }


        public async Task<ProjectDto> AddAsync(ProjectCreateDto dto)
        {
            var project = MapToNewProject(dto);

            await _ProjectRepository.AddAsync(project);

            await _memberService.AddAsync(new MemberCreateDto
            {
                UserId = dto.UserCreatedId,
                ProjectId = project.Id
            });

            var item = new Project_Created_Event(project.Id, Guid.Empty, project.Name, project.Status.ToString());

            await _eventBus.PublishAsync(item);

            return MapToDto(project);
        }

        public async Task UpdateAsync(ProjectUpdateDto dto)
        {
            var project = await _ProjectRepository.GetByIdAsync(dto.Id);

            if (project == null) 
                throw new BusinessException("Project not found");

            MapUpdate(dto, project);

            await _ProjectRepository.UpdateAsync(project);
        }

        public async Task DeleteAsync(Guid id)
        {
            var project = await _ProjectRepository.GetByIdAsync(id);

            if (project == null) 
                throw new BusinessException("Project not found");
                
            await _ProjectRepository.DeleteAsync(id);
        }


        private static void MapUpdate(ProjectUpdateDto dto, Domain.Entities.Project Project)
        {
            Project.Name = dto.Name;
            Project.Description = dto.Description;
            Project.UserCreatedId = dto.UserCreatedId;
            Project.Status = dto.Status;
            Project.UpdatedAt = DateTime.UtcNow;
        }

        private Domain.Entities.Project MapToNewProject(ProjectCreateDto dto)
        {
            return new Market.Domain.Entities.Project
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                UserCreatedId = dto.UserCreatedId,
                Status = StatusEnum.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        private ProjectDto MapToDto(Market.Domain.Entities.Project Project)
        {
            return new ProjectDto
            {
                Id = Project.Id,
                Name = Project.Name,
                Description = Project.Description,
                UserCreatedId = Project.UserCreatedId,
                CreatedAt = Project.CreatedAt,
                UpdatedAt = Project.UpdatedAt,
                Status = Project.Status
            };
        }
    }
}
