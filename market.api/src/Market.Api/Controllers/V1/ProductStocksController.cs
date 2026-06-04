using Market.Api.Helpers;
using Market.Application.DTOs;
using Market.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Market.Api.Controllers.V1;

[Route("v1/[controller]")]
public class ProductStocksController : CrudController<ProductStockDto, ProductStockCreateDto, ProductStockUpdateDto>
{
    public ProductStocksController(IProductStockService service, IErrorLogService errorLogService, ILogger<ProductStocksController> logger)
        : base(service, errorLogService, logger, "ProductStock")
    {
    }

    [HttpGet]
    [AuthorizeRole(RoleConstants.ProductStockRole.List)]
    public override Task<ActionResult> GetPaged([FromQuery] PaginationRequestDto request) => base.GetPaged(request);

    [HttpGet("{id:guid}")]
    [AuthorizeRole(RoleConstants.ProductStockRole.List)]
    public override Task<ActionResult> GetById(Guid id) => base.GetById(id);

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
