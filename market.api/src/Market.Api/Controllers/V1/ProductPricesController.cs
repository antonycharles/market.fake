using Market.Api.Helpers;
using Market.Application.DTOs;
using Market.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Market.Api.Controllers.V1;

[Route("v1/[controller]")]
public class ProductPricesController : CrudController<ProductPriceDto, ProductPriceCreateDto, ProductPriceUpdateDto>
{
    public ProductPricesController(IProductPriceService service, IErrorLogService errorLogService, ILogger<ProductPricesController> logger)
        : base(service, errorLogService, logger, "ProductPrice")
    {
    }

    [HttpGet]
    [AuthorizeRole(RoleConstants.ProductPriceRole.List)]
    public override Task<ActionResult> GetPaged([FromQuery] PaginationRequestDto request) => base.GetPaged(request);

    [HttpGet("{id:guid}")]
    [AuthorizeRole(RoleConstants.ProductPriceRole.List)]
    public override Task<ActionResult> GetById(Guid id) => base.GetById(id);

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
