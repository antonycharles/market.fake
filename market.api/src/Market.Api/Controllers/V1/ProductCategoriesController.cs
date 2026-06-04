using Market.Api.Helpers;
using Market.Application.DTOs;
using Market.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Market.Api.Controllers.V1;

[Route("v1/[controller]")]
public class ProductCategoriesController : CrudController<ProductCategoryDto, ProductCategoryCreateDto, ProductCategoryUpdateDto>
{
    public ProductCategoriesController(IProductCategoryService service, IErrorLogService errorLogService, ILogger<ProductCategoriesController> logger)
        : base(service, errorLogService, logger, "ProductCategory")
    {
    }

    [HttpGet]
    [AuthorizeRole(RoleConstants.ProductCategoryRole.List)]
    public override Task<ActionResult> GetPaged([FromQuery] PaginationRequestDto request) => base.GetPaged(request);

    [HttpGet("{id:guid}")]
    [AuthorizeRole(RoleConstants.ProductCategoryRole.List)]
    public override Task<ActionResult> GetById(Guid id) => base.GetById(id);

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
