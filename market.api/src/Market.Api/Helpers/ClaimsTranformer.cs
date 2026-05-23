using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace Market.Api.Helpers
{
    public class ClaimsTranformer : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var customPrincipal = new CustomClaimsPrincipal(principal.Identity) as ClaimsPrincipal;
            return Task.FromResult(customPrincipal);
        }
    }
}