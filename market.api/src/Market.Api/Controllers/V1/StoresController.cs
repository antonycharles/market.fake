using Market.Api.Helpers;
using Market.Application.DTOs;
using Market.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Market.Api.Controllers.V1;

[Route("v1/[controller]")]
public class StoresController : CrudController<StoreDto, StoreCreateDto, StoreUpdateDto>
{
    public StoresController(IStoreService service, IErrorLogService errorLogService, ILogger<StoresController> logger)
        : base(service, errorLogService, logger, "Store")
    {
    }

    [HttpGet]
    [AuthorizeRole(RoleConstants.StoreRole.List)]
    public override Task<ActionResult> GetPaged([FromQuery] PaginationRequestDto request) => base.GetPaged(request);

    [HttpGet("{id:guid}")]
    [AuthorizeRole(RoleConstants.StoreRole.List)]
    public override Task<ActionResult> GetById(Guid id) => base.GetById(id);

    [HttpPost]
    [AuthorizeRole(RoleConstants.StoreRole.Create)]
    public override Task<ActionResult> Create([FromBody] StoreCreateDto dto) => base.Create(dto);

    [HttpPut("{id:guid}")]
    [AuthorizeRole(RoleConstants.StoreRole.Update)]
    public override Task<ActionResult> Update(Guid id, [FromBody] StoreUpdateDto dto) => base.Update(id, dto);

    [HttpDelete("{id:guid}")]
    [AuthorizeRole(RoleConstants.StoreRole.Delete)]
    public override Task<ActionResult> Delete(Guid id) => base.Delete(id);
}
