using Market.Api.Helpers;
using Market.Application.DTOs;
using Market.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Market.Api.Controllers.V1;

[Route("v1/[controller]")]
public class ProductInformationsController : CrudController<ProductInformationDto, ProductInformationCreateDto, ProductInformationUpdateDto>
{
    public ProductInformationsController(IProductInformationService service, IErrorLogService errorLogService, ILogger<ProductInformationsController> logger)
        : base(service, errorLogService, logger, "ProductInformation")
    {
    }

    [HttpGet]
    [AuthorizeRole(RoleConstants.ProductInformationRole.List)]
    public override Task<ActionResult> GetPaged([FromQuery] PaginationRequestDto request) => base.GetPaged(request);

    [HttpGet("{id:guid}")]
    [AuthorizeRole(RoleConstants.ProductInformationRole.List)]
    public override Task<ActionResult> GetById(Guid id) => base.GetById(id);

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
