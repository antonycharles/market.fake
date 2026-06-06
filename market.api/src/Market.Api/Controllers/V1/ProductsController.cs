using Market.Api.Helpers;
using Market.Application.DTOs;
using Market.Application.Interfaces;
using Market.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Market.Api.Controllers.V1;

[Route("v1/[controller]")]
public class ProductsController : CrudController<ProductDto, ProductCreateDto, ProductUpdateDto>
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService service, IErrorLogService errorLogService, ILogger<ProductsController> logger)
        : base(service, errorLogService, logger, "Product")
    {
        _productService = service;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public override async Task<ActionResult> GetPaged([FromQuery] PaginationRequestDto request)
    {
        try
        {
            var result = await _productService.GetProductListAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return await HandleExceptionAsync(ex, "Error fetching products");
        }
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public override Task<ActionResult> GetById(Guid id) => base.GetById(id);

    [HttpGet("code/{code:int}")]
    [AllowAnonymous]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult> GetByCode(int code)
    {
        try
        {
            var result = await _productService.GetByCodeAsync(code);
            return Ok(result);
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, "Product business error");
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return await HandleExceptionAsync(ex, "Error fetching Product by code");
        }
    }

    [HttpPost]
    [AuthorizeRole(RoleConstants.ProductRole.Create)]
    public override Task<ActionResult> Create([FromBody] ProductCreateDto dto) => base.Create(dto);

    [HttpPut("{id:guid}")]
    [AuthorizeRole(RoleConstants.ProductRole.Update)]
    public override Task<ActionResult> Update(Guid id, [FromBody] ProductUpdateDto dto) => base.Update(id, dto);

    [HttpDelete("{id:guid}")]
    [AuthorizeRole(RoleConstants.ProductRole.Delete)]
    public override Task<ActionResult> Delete(Guid id) => base.Delete(id);
}
