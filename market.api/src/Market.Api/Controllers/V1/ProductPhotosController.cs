using Market.Api.Helpers;
using Market.Application.DTOs;
using Market.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Market.Api.Controllers.V1;

[Route("v1/[controller]")]
public class ProductPhotosController : CrudController<ProductPhotoDto, ProductPhotoCreateDto, ProductPhotoUpdateDto>
{
    private readonly IProductPhotoService _productPhotoService;
    private readonly IErrorLogService _errorLogService;
    private readonly ILogger<ProductPhotosController> _logger;

    public ProductPhotosController(IProductPhotoService service, IErrorLogService errorLogService, ILogger<ProductPhotosController> logger)
        : base(service, errorLogService, logger, "ProductPhoto")
    {
        _productPhotoService = service;
        _errorLogService = errorLogService;
        _logger = logger;
    }

    [HttpGet]
    [AuthorizeRole(RoleConstants.ProductPhotoRole.List)]
    public override Task<ActionResult> GetPaged([FromQuery] PaginationRequestDto request) => base.GetPaged(request);

    [HttpGet("{id:guid}")]
    [AuthorizeRole(RoleConstants.ProductPhotoRole.List)]
    public override Task<ActionResult> GetById(Guid id) => base.GetById(id);

    [HttpGet("product/{productId:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult> GetByProductId(Guid productId)
    {
        try
        {
            var result = await _productPhotoService.GetByProductIdAsync(productId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching product photos by product id");
            await _errorLogService.AddAsync(ex, "Error fetching product photos by product id", HttpContext.Request.Path, HttpContext.Request.Method);
            return StatusCode(500, "Internal server error");
        }
    }

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
