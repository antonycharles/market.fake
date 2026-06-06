using Market.Api.Helpers;
using Market.Application.DTOs;
using Market.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Market.Api.Controllers.V1;

[Route("v1/[controller]")]
public class ProductPricesController : CrudController<ProductPriceDto, ProductPriceCreateDto, ProductPriceUpdateDto>
{
    private readonly IProductPriceService _productPriceService;
    private readonly IErrorLogService _errorLogService;
    private readonly ILogger<ProductPricesController> _logger;

    public ProductPricesController(IProductPriceService service, IErrorLogService errorLogService, ILogger<ProductPricesController> logger)
        : base(service, errorLogService, logger, "ProductPrice")
    {
        _productPriceService = service;
        _errorLogService = errorLogService;
        _logger = logger;
    }

    [HttpGet]
    [AuthorizeRole(RoleConstants.ProductPriceRole.List)]
    public override Task<ActionResult> GetPaged([FromQuery] PaginationRequestDto request) => base.GetPaged(request);

    [HttpGet("{id:guid}")]
    [AuthorizeRole(RoleConstants.ProductPriceRole.List)]
    public override Task<ActionResult> GetById(Guid id) => base.GetById(id);

    [HttpGet("product/{productId:guid}/current")]
    [AllowAnonymous]
    public async Task<ActionResult> GetCurrentByProductId(Guid productId)
    {
        try
        {
            var result = await _productPriceService.GetCurrentByProductIdAsync(productId);

            return result is null ? NotFound("Current product price not found") : Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching current product price by product id");
            await _errorLogService.AddAsync(ex, "Error fetching current product price by product id", HttpContext.Request.Path, HttpContext.Request.Method);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    [AuthorizeRole(RoleConstants.ProductPriceRole.Create)]
    public override Task<ActionResult> Create([FromBody] ProductPriceCreateDto dto) => base.Create(dto);

    [HttpPut("{id:guid}")]
    [AuthorizeRole(RoleConstants.ProductPriceRole.Update)]
    public override Task<ActionResult> Update(Guid id, [FromBody] ProductPriceUpdateDto dto) => base.Update(id, dto);

    [HttpDelete("{id:guid}")]
    [AuthorizeRole(RoleConstants.ProductPriceRole.Delete)]
    public override Task<ActionResult> Delete(Guid id) => base.Delete(id);
}
