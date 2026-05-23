using Microsoft.AspNetCore.Mvc;
using Market.Application.Interfaces;
using Market.Application.DTOs;
using Market.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Market.Api.Helpers;
using Market.Api.Extensions;

namespace Market.Api.Controllers.V1;

[ApiController]
[Route("v1/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class MemberController : ControllerBase
{
    private readonly ILogger<MemberController> _logger;
    private readonly IMemberService _memberService;

    public MemberController(ILogger<MemberController> logger, IMemberService memberService)
    {
        _logger = logger;
        _memberService = memberService;
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MemberDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [AuthorizeRole(RoleConstants.MemberRole.List)]
    public async Task<ActionResult<MemberDto>> GetById(Guid id)
    {
        try
        {
            var member = await _memberService.GetByIdAsync(id);
            if (member == null)
                return NotFound("Member not found");
            return Ok(member);
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, "Business error");
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching member by id");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(MemberDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    [AuthorizeRole(RoleConstants.MemberRole.Create)]
    public async Task<ActionResult<MemberDto>> Create([FromBody] MemberCreateDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _memberService.AddAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, "Business error");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating member");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MemberDto>), 200)]
    [ProducesResponseType(500)]
    [AuthorizeRole(RoleConstants.MemberRole.List)]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetByProjectIdAsync([FromQuery] Guid projectId)
    {
        try
        {
            var members = await _memberService.GetByProjectIdAsync(projectId);
            return Ok(members);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching members by project id");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [AuthorizeRole(RoleConstants.MemberRole.Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _memberService.DeleteAsync(id);
            return NoContent();
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, "Business error");
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting member");
            return StatusCode(500, "Internal server error");
        }
    }
}