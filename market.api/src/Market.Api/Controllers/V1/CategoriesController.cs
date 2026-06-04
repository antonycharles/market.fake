using Market.Api.Helpers;
using Market.Application.DTOs;
using Market.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Market.Api.Controllers.V1;

[Route("v1/[controller]")]
public class CategoriesController : CrudController<CategoryDto, CategoryCreateDto, CategoryUpdateDto>
{
    public CategoriesController(ICategoryService service, IErrorLogService errorLogService, ILogger<CategoriesController> logger)
        : base(service, errorLogService, logger, "Category")
    {
    }

    [HttpGet]
    [AllowAnonymous]
    public override Task<ActionResult> GetPaged([FromQuery] PaginationRequestDto request) => base.GetPaged(request);

    [HttpGet("{id:guid}")]
    [AuthorizeRole(RoleConstants.CategoryRole.List)]
    public override Task<ActionResult> GetById(Guid id) => base.GetById(id);

    [HttpPost]
    [AuthorizeRole(RoleConstants.CategoryRole.Create)]
    public override Task<ActionResult> Create([FromBody] CategoryCreateDto dto) => base.Create(dto);

    [HttpPut("{id:guid}")]
    [AuthorizeRole(RoleConstants.CategoryRole.Update)]
    public override Task<ActionResult> Update(Guid id, [FromBody] CategoryUpdateDto dto) => base.Update(id, dto);

    [HttpDelete("{id:guid}")]
    [AuthorizeRole(RoleConstants.CategoryRole.Delete)]
    public override Task<ActionResult> Delete(Guid id) => base.Delete(id);
}
