using Market.Api.Helpers;
using Market.Application.DTOs;
using Market.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Market.Api.Controllers.V1;

[Route("v1/[controller]")]
public class ProductPhotosController : CrudController<ProductPhotoDto, ProductPhotoCreateDto, ProductPhotoUpdateDto>
{
    public ProductPhotosController(IProductPhotoService service, IErrorLogService errorLogService, ILogger<ProductPhotosController> logger)
        : base(service, errorLogService, logger, "ProductPhoto")
    {
    }

    [HttpGet]
    [AuthorizeRole(RoleConstants.ProductPhotoRole.List)]
    public override Task<ActionResult> GetPaged([FromQuery] PaginationRequestDto request) => base.GetPaged(request);

    [HttpGet("{id:guid}")]
    [AuthorizeRole(RoleConstants.ProductPhotoRole.List)]
    public override Task<ActionResult> GetById(Guid id) => base.GetById(id);

    [HttpPost]
    [AuthorizeRole(RoleConstants.ProductPhotoRole.Create)]
    public override Task<ActionResult> Create([FromBody] ProductPhotoCreateDto dto) => base.Create(dto);

    [HttpPut("{id:guid}")]
    [AuthorizeRole(RoleConstants.ProductPhotoRole.Update)]
    public override Task<ActionResult> Update(Guid id, [FromBody] ProductPhotoUpdateDto dto) => base.Update(id, dto);

    [HttpDelete("{id:guid}")]
    [AuthorizeRole(RoleConstants.ProductPhotoRole.Delete)]
    public override Task<ActionResult> Delete(Guid id) => base.Delete(id);
}
