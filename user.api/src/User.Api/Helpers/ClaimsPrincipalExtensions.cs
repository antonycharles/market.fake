using System.Security.Claims;
using User.Application.Extensions;

namespace User.Api.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid? GetUserId(this ClaimsPrincipal principal)
        {
            var userId = principal.FindFirst(CustomClaimTypes.Id)?.Value
                ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? principal.FindFirst("sub")?.Value;

            return Guid.TryParse(userId, out var id) ? id : null;
        }
    }
}
