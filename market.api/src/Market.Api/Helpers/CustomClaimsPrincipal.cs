using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Market.Api.Extensions;

namespace Market.Api.Helpers
{
    public class CustomClaimsPrincipal : ClaimsPrincipal
    {
        public CustomClaimsPrincipal(IIdentity identity) : base(identity)
        {
        }

        public override bool IsInRole(string role)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Claims.Any(c => c.Type == CustomClaimTypes.Role && c.Value.Equals(role, StringComparison.OrdinalIgnoreCase));
        }
    }
}