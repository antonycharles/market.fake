using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using User.Core.Handlers;

namespace User.Api.Helpers
{
    public class CustomJwtBearerHandler : JwtBearerHandler
    {
        private readonly ITokenHandler _tokenHandler;

        public CustomJwtBearerHandler(
            ITokenHandler tokenHandler,
            IOptionsMonitor<JwtBearerOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _tokenHandler = tokenHandler;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Context.Request.Headers.TryGetValue("Authorization", out var authorizationHeaderValues))
                return Task.FromResult(AuthenticateResult.Fail("Authorization header not found."));

            var authorizationHeader = authorizationHeaderValues.FirstOrDefault();
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
                return Task.FromResult(AuthenticateResult.Fail("Bearer token not found in Authorization header."));

            var token = authorizationHeader.Substring("Bearer ".Length).Trim();

            if (_tokenHandler.ValidateToken(token) == false)
                return Task.FromResult(AuthenticateResult.Fail("Token validation failed."));

            var principal = GetClaims(token);
            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, JwtBearerDefaults.AuthenticationScheme)));
        }

        private static ClaimsPrincipal GetClaims(string tokenValue)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadToken(tokenValue) as JwtSecurityToken;
            var claimsIdentity = new ClaimsIdentity(token.Claims, JwtBearerDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(claimsIdentity);
        }
    }
}
