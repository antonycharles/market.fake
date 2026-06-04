using Market.Api.Helpers;
using Market.Application.DTOs;
using Market.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Market.Api.Controllers.V1;

[Route("v1/[controller]")]
public class ProductsController : CrudController<ProductDto, ProductCreateDto, ProductUpdateDto>
{
    private readonly IProductService _productService;

    public ProductsController(IProductService service, IErrorLogService errorLogService, ILogger<ProductsController> logger)
        : base(service, errorLogService, logger, "Product")
    {
        _productService = service;
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
    [AuthorizeRole(RoleConstants.ProductRole.List)]
    public override Task<ActionResult> GetById(Guid id) => base.GetById(id);

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
