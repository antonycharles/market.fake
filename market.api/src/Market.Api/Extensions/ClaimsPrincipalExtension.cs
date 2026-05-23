using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Market.Api.Extensions
{
    public static class ClaimsPrincipalExtension
    {

        public static Guid UserId(this ClaimsPrincipal claimsPrincipal)
        {
            var idClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.Id);
            return idClaim?.Value != null ? new Guid(idClaim.Value) : Guid.Empty;
        }

        public static string GetName(this ClaimsPrincipal claimsPrincipal)
        {
            var nameClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.Name);
            return nameClaim?.Value ?? "Unknown User";
        }

        public static string GetEmail(this ClaimsPrincipal claimsPrincipal)
        {
            var emailClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.Email);
            return emailClaim?.Value ?? "Unknown Email";
        }
        public static string? GetImage(this ClaimsPrincipal claimsPrincipal)
        {
            var imageClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.Image);


            if(imageClaim?.Value != null && imageClaim.Value.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                return imageClaim.Value;
            }

            return  claimsPrincipal.GetName().Substring(0,1).ToUpper();
        }
    }
}