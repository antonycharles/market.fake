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
    public class UserCreditCardController : ControllerBase
    {
        private readonly IUserCreditCardHandler _userCreditCardHandler;

        public UserCreditCardController(IUserCreditCardHandler userCreditCardHandler)
        {
            _userCreditCardHandler = userCreditCardHandler;
        }

        [HttpGet("user/{userId}")]
        [AuthorizeRole(RoleConstants.UserCreditCardRole.List)]
        [ProducesResponseType(typeof(List<UserCreditCardResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByUserIdAsync(Guid userId)
        {
            try
            {
                var response = await _userCreditCardHandler.GetByUserIdAsync(userId);
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
        [AuthorizeRole(RoleConstants.UserCreditCardRole.MeList)]
        [ProducesResponseType(typeof(List<UserCreditCardResponse>), StatusCodes.Status200OK)]
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

                var response = await _userCreditCardHandler.GetByUserIdAsync(userId.Value);
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
        [AuthorizeRole(RoleConstants.UserCreditCardRole.List)]
        [ProducesResponseType(typeof(UserCreditCardResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            try
            {
                var response = await _userCreditCardHandler.GetByIdAsync(id);
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

        [HttpGet("me/{id}")]
        [AuthorizeRole(RoleConstants.UserCreditCardRole.MeList)]
        [ProducesResponseType(typeof(UserCreditCardResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByIdMeAsync(Guid id)
        {
            try
            {
                var userId = User.GetUserId();

                if (!userId.HasValue)
                    return Unauthorized();
                    
                var response = await _userCreditCardHandler.GetByIdMeAsync(id, userId.Value);
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
        [AuthorizeRole(RoleConstants.UserCreditCardRole.Create, RoleConstants.UserCreditCardRole.Update)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateOrCreateAsync([FromBody] UserCreditCardRequest request)
        {
            try
            {
                if (request == null || !ModelState.IsValid)
                    return BadRequest(ModelState);

                await _userCreditCardHandler.UpdateOrCreateAsync(request);
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
        [AuthorizeRole(RoleConstants.UserCreditCardRole.MeCreate, RoleConstants.UserCreditCardRole.MeUpdate)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateOrCreateMeAsync([FromBody] UserCreditCardRequest request)
        {
            try
            {
                if (request == null || !ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = User.GetUserId();
                if (!userId.HasValue)
                    return Unauthorized();

                request.UserId = userId.Value;
                await _userCreditCardHandler.UpdateOrCreateAsync(request);
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
        [AuthorizeRole(RoleConstants.UserCreditCardRole.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            try
            {
                await _userCreditCardHandler.DeleteAsync(id);
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
        [AuthorizeRole(RoleConstants.UserCreditCardRole.MeDelete)]
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

                await _userCreditCardHandler.DeleteMeAsync(id, userId.Value);
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
