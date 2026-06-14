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
    public class UserAddressController : ControllerBase
    {
        private readonly IUserAddressHandler _userAddressHandler;

        public UserAddressController(IUserAddressHandler userAddressHandler)
        {
            _userAddressHandler = userAddressHandler;
        }

        [HttpGet("user/{userId}")]
        [AuthorizeRole(RoleConstants.UserAddressRole.List)]
        [ProducesResponseType(typeof(List<UserAddressResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByUserIdAsync(Guid userId)
        {
            try
            {
                var response = await _userAddressHandler.GetByUserIdAsync(userId);
                return Ok(response);
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

        [HttpGet("me")]
        [AuthorizeRole(RoleConstants.UserAddressRole.MeList)]
        [ProducesResponseType(typeof(List<UserAddressResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMeAsync()
        {
            try
            {
                var userId = User.GetUserId();
                if (!userId.HasValue)
                    return Unauthorized();

                var response = await _userAddressHandler.GetByUserIdAsync(userId.Value);
                return Ok(response);
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

        [HttpGet("{id}")]
        [AuthorizeRole(RoleConstants.UserAddressRole.List)]
        [ProducesResponseType(typeof(UserAddressResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            try
            {
                var response = await _userAddressHandler.GetByIdAsync(id);
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
        [AuthorizeRole(RoleConstants.UserAddressRole.Create, RoleConstants.UserAddressRole.Update)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateOrCreateAsync([FromBody] UserAddressRequest request)
        {
            try
            {
                if (request == null || !ModelState.IsValid)
                    return BadRequest(ModelState);

                await _userAddressHandler.UpdateOrCreateAsync(request);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return Problem(ex.Message, statusCode: StatusCodes.Status404NotFound);
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

        [HttpPost("me")]
        [AuthorizeRole(RoleConstants.UserAddressRole.MeCreate, RoleConstants.UserAddressRole.MeUpdate)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateOrCreateMeAsync([FromBody] UserAddressRequest request)
        {
            try
            {
                if (request == null || !ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = User.GetUserId();
                if (!userId.HasValue)
                    return Unauthorized();

                request.UserId = userId.Value;
                await _userAddressHandler.UpdateOrCreateAsync(request);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return Problem(ex.Message, statusCode: StatusCodes.Status404NotFound);
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

        [HttpDelete("{id}")]
        [AuthorizeRole(RoleConstants.UserAddressRole.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            try
            {
                await _userAddressHandler.DeleteAsync(id);
                return Ok();
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

        [HttpDelete("me/{id}")]
        [AuthorizeRole(RoleConstants.UserAddressRole.MeDelete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteMeAsync(Guid id)
        {
            try
            {
                var userId = User.GetUserId();
                if (!userId.HasValue)
                    return Unauthorized();

                await _userAddressHandler.DeleteMeAsync(id, userId.Value);
                return Ok();
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
    }
}
