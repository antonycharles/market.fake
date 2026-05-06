using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace User.Api.Helpers
{
    public class ClaimsTranformer : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var customPrincipal = new CustomClaimsPrincipal(principal.Identity);
            return Task.FromResult(customPrincipal as ClaimsPrincipal);
        }
    }
}
