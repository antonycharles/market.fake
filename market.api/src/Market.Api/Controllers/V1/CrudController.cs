using Market.Application.DTOs;
using Market.Application.Interfaces;
using Market.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Market.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
public abstract class CrudController<TDto, TCreateDto, TUpdateDto> : ControllerBase
{
    private readonly ICrudService<TDto, TCreateDto, TUpdateDto> _service;
    private readonly IErrorLogService _errorLogService;
    private readonly ILogger _logger;
    private readonly string _entityName;

    protected CrudController(
        ICrudService<TDto, TCreateDto, TUpdateDto> service,
        IErrorLogService errorLogService,
        ILogger logger,
        string entityName)
    {
        _service = service;
        _errorLogService = errorLogService;
        _logger = logger;
        _entityName = entityName;
    }

    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public virtual async Task<ActionResult> GetPaged([FromQuery] PaginationRequestDto request)
    {
        try
        {
            var result = await _service.GetPagedAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return await HandleExceptionAsync(ex, $"Error fetching {_entityName}");
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public virtual async Task<ActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(result);
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, "{EntityName} business error", _entityName);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return await HandleExceptionAsync(ex, $"Error fetching {_entityName} by id");
        }
    }

    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public virtual async Task<ActionResult> Create([FromBody] TCreateDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return StatusCode(500, ModelState);

            var result = await _service.AddAsync(dto);
            return Ok(result);
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, "{EntityName} business error", _entityName);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return await HandleExceptionAsync(ex, $"Error creating {_entityName}");
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public virtual async Task<ActionResult> Update(Guid id, [FromBody] TUpdateDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return StatusCode(500, ModelState);

            var dtoId = dto?.GetType().GetProperty("Id")?.GetValue(dto) as Guid?;

            if (dtoId != id)
                return NotFound("Id mismatch");

            var result = await _service.UpdateAsync(dto);
            return Ok(result);
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, "{EntityName} business error", _entityName);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return await HandleExceptionAsync(ex, $"Error updating {_entityName}");
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public virtual async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return Ok();
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, "{EntityName} business error", _entityName);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return await HandleExceptionAsync(ex, $"Error deleting {_entityName}");
        }
    }

    protected async Task<ActionResult> HandleExceptionAsync(Exception ex, string logMessage)
    {
        _logger.LogError(ex, logMessage);
        await TrySaveErrorLogAsync(ex, logMessage);
        return StatusCode(500, "Internal server error");
    }

    private async Task TrySaveErrorLogAsync(Exception ex, string source)
    {
        try
        {
            await _errorLogService.AddAsync(ex, source, HttpContext.Request.Path, HttpContext.Request.Method);
        }
        catch (Exception logException)
        {
            _logger.LogError(logException, "Error saving error log");
        }
    }
}
