using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using User.Api.Helpers;
using User.Core.Exceptions;
using User.Core.Handlers;
using User.Core.Requests;
using User.Core.Responses;

namespace User.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class UserPhotoController : ControllerBase
    {
        private readonly IUserPhotoHandler _userPhotoHandler;

        public UserPhotoController(IUserPhotoHandler userPhotoHandler)
        {
            _userPhotoHandler = userPhotoHandler;
        }

        [HttpGet("{id}")]
        [AuthorizeRole(RoleConstants.UserRole.List)]
        [ProducesResponseType(typeof(UserPhotoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            try
            {
                var response = await _userPhotoHandler.GetByIdAsync(id);
                return Ok(response);
            }
            catch (NotFoundException ex)
            {
                return Problem(ex.Message, statusCode: StatusCodes.Status404NotFound);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [AuthorizeRole(RoleConstants.UserRole.Create, RoleConstants.UserRole.Update)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateOrCreateAsync([FromBody] UserPhotoRequest request)
        {
            try
            {
                if (request == null || !ModelState.IsValid)
                    return BadRequest(ModelState);

                await _userPhotoHandler.UpdateOrCreateAsync(request);
                return NoContent();
            }
            catch (BusinessException ex)
            {
                return Problem(ex.Message, statusCode: StatusCodes.Status400BadRequest);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}
