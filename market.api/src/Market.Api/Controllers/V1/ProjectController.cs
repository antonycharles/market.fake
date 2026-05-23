using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Market.Application.Interfaces;
using Market.Application.DTOs;
using Market.Domain.Exceptions;
using Market.Api.Helpers;
using Market.Api.Extensions;

namespace Market.Api.Controllers.V1;

[ApiController]
[Route("v1/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class ProjectController : ControllerBase
{
    private readonly ILogger<ProjectController> _logger;
    private readonly IProjectService _ProjectService;

    public ProjectController(ILogger<ProjectController> logger, IProjectService ProjectService)
    {
        _logger = logger;
        _ProjectService = ProjectService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProjectDto>), 200)]
    [ProducesResponseType(500)]
    [AuthorizeRole(RoleConstants.ProjectRole.List)]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetAll()
    {
        try
        {
            var projects = await _ProjectService.GetAllAsync();
            return Ok(projects);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching projects");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProjectDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [AuthorizeRole(RoleConstants.ProjectRole.List)]
    public async Task<ActionResult<ProjectDto>> GetById(Guid id)
    {
        try
        {
            var Project = await _ProjectService.GetByIdAsync(id);
            
            if (Project == null)
                return NotFound("Project not found");
            return Ok(Project);
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, "Business error");
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Project by id");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProjectDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    [AuthorizeRole(RoleConstants.ProjectRole.Create)]
    public async Task<ActionResult<ProjectDto>> Create([FromBody] ProjectCreateDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            dto.UserCreatedId = User.UserId();

            var created = await _ProjectService.AddAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, "Business error");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Project");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [AuthorizeRole(RoleConstants.ProjectRole.Update)]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProjectUpdateDto dto)
    {
        try
        {
            if (id != dto.Id)
                return BadRequest("Id mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            dto.UserCreatedId = User.UserId();

            await _ProjectService.UpdateAsync(dto);
            
            return NoContent();
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, "Business error");
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating Project");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [AuthorizeRole(RoleConstants.ProjectRole.Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _ProjectService.DeleteAsync(id);
            return NoContent();
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, "Business error");
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting Project");
            return StatusCode(500, "Internal server error");
        }
    }
}
