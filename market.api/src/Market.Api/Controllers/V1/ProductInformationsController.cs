using Market.Api.Helpers;
using Market.Application.DTOs;
using Market.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Market.Api.Controllers.V1;

[Route("v1/[controller]")]
public class ProductInformationsController : CrudController<ProductInformationDto, ProductInformationCreateDto, ProductInformationUpdateDto>
{
    private readonly IProductInformationService _productInformationService;
    private readonly IErrorLogService _errorLogService;
    private readonly ILogger<ProductInformationsController> _logger;

    public ProductInformationsController(IProductInformationService service, IErrorLogService errorLogService, ILogger<ProductInformationsController> logger)
        : base(service, errorLogService, logger, "ProductInformation")
    {
        _productInformationService = service;
        _errorLogService = errorLogService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public override Task<ActionResult> GetPaged([FromQuery] PaginationRequestDto request) => base.GetPaged(request);

    [HttpGet("{id:guid}")]
    [AuthorizeRole(RoleConstants.ProductInformationRole.List)]
    public override Task<ActionResult> GetById(Guid id) => base.GetById(id);

    [HttpGet("product/{productId:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult> GetByProductId(Guid productId)
    {
        try
        {
            var result = await _productInformationService.GetByProductIdAsync(productId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching product informations by product id");
            await _errorLogService.AddAsync(ex, "Error fetching product informations by product id", HttpContext.Request.Path, HttpContext.Request.Method);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    [AuthorizeRole(RoleConstants.ProductInformationRole.Create)]
    public override Task<ActionResult> Create([FromBody] ProductInformationCreateDto dto) => base.Create(dto);

    [HttpPut("{id:guid}")]
    [AuthorizeRole(RoleConstants.ProductInformationRole.Update)]
    public override Task<ActionResult> Update(Guid id, [FromBody] ProductInformationUpdateDto dto) => base.Update(id, dto);

    [HttpDelete("{id:guid}")]
    [AuthorizeRole(RoleConstants.ProductInformationRole.Delete)]
    public override Task<ActionResult> Delete(Guid id) => base.Delete(id);
}
