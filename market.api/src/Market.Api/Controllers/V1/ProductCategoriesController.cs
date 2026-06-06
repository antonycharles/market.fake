using Market.Api.Helpers;
using Market.Application.DTOs;
using Market.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Market.Api.Controllers.V1;

[Route("v1/[controller]")]
public class ProductCategoriesController : CrudController<ProductCategoryDto, ProductCategoryCreateDto, ProductCategoryUpdateDto>
{
    private readonly IProductCategoryService _productCategoryService;
    private readonly IErrorLogService _errorLogService;
    private readonly ILogger<ProductCategoriesController> _logger;

    public ProductCategoriesController(IProductCategoryService service, IErrorLogService errorLogService, ILogger<ProductCategoriesController> logger)
        : base(service, errorLogService, logger, "ProductCategory")
    {
        _productCategoryService = service;
        _errorLogService = errorLogService;
        _logger = logger;
    }

    [HttpGet]
    [AuthorizeRole(RoleConstants.ProductCategoryRole.List)]
    public override Task<ActionResult> GetPaged([FromQuery] PaginationRequestDto request) => base.GetPaged(request);

    [HttpGet("{id:guid}")]
    [AuthorizeRole(RoleConstants.ProductCategoryRole.List)]
    public override Task<ActionResult> GetById(Guid id) => base.GetById(id);

    [HttpGet("product/{productId:guid}")]
    [AuthorizeRole(RoleConstants.ProductCategoryRole.List)]
    public async Task<ActionResult> GetByProductId(Guid productId)
    {
        try
        {
            var result = await _productCategoryService.GetCategoriesByProductIdAsync(productId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching product categories by product id");
            await _errorLogService.AddAsync(ex, "Error fetching product categories by product id", HttpContext.Request.Path, HttpContext.Request.Method);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    [AuthorizeRole(RoleConstants.ProductCategoryRole.Create)]
    public override Task<ActionResult> Create([FromBody] ProductCategoryCreateDto dto) => base.Create(dto);

    [HttpPut("{id:guid}")]
    [AuthorizeRole(RoleConstants.ProductCategoryRole.Update)]
    public override Task<ActionResult> Update(Guid id, [FromBody] ProductCategoryUpdateDto dto) => base.Update(id, dto);

    [HttpDelete("{id:guid}")]
    [AuthorizeRole(RoleConstants.ProductCategoryRole.Delete)]
    public override Task<ActionResult> Delete(Guid id) => base.Delete(id);
}
