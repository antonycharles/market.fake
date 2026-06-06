using Market.Api.Helpers;
using Market.Application.DTOs;
using Market.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Market.Api.Controllers.V1;

[Route("v1/[controller]")]
public class ProductStocksController : CrudController<ProductStockDto, ProductStockCreateDto, ProductStockUpdateDto>
{
    private readonly IProductStockService _productStockService;
    private readonly IErrorLogService _errorLogService;
    private readonly ILogger<ProductStocksController> _logger;

    public ProductStocksController(IProductStockService service, IErrorLogService errorLogService, ILogger<ProductStocksController> logger)
        : base(service, errorLogService, logger, "ProductStock")
    {
        _productStockService = service;
        _errorLogService = errorLogService;
        _logger = logger;
    }

    [HttpGet]
    [AuthorizeRole(RoleConstants.ProductStockRole.List)]
    public override Task<ActionResult> GetPaged([FromQuery] PaginationRequestDto request) => base.GetPaged(request);

    [HttpGet("{id:guid}")]
    [AuthorizeRole(RoleConstants.ProductStockRole.List)]
    public override Task<ActionResult> GetById(Guid id) => base.GetById(id);

    [HttpGet("product/{productId:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult> GetByProductId(Guid productId)
    {
        try
        {
            var result = await _productStockService.GetByProductIdAsync(productId);

            return result is null ? NotFound("Product stock not found") : Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching product stock by product id");
            await _errorLogService.AddAsync(ex, "Error fetching product stock by product id", HttpContext.Request.Path, HttpContext.Request.Method);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    [AuthorizeRole(RoleConstants.ProductStockRole.Create)]
    public override Task<ActionResult> Create([FromBody] ProductStockCreateDto dto) => base.Create(dto);

    [HttpPut("{id:guid}")]
    [AuthorizeRole(RoleConstants.ProductStockRole.Update)]
    public override Task<ActionResult> Update(Guid id, [FromBody] ProductStockUpdateDto dto) => base.Update(id, dto);

    [HttpDelete("{id:guid}")]
    [AuthorizeRole(RoleConstants.ProductStockRole.Delete)]
    public override Task<ActionResult> Delete(Guid id) => base.Delete(id);
}
