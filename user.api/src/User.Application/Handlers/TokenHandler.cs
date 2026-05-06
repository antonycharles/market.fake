using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using User.Core.Handlers;

namespace User.Application.Handlers
{
    public class TokenHandler : ITokenHandler
    {
        private const string T_ISSUER = "accounts.api";
        private readonly ITokenKeyHandler _tokenKeyHandler;

        public TokenHandler(ITokenKeyHandler tokenKeyHandler)
        {
            _tokenKeyHandler = tokenKeyHandler;
        }

        public bool ValidateToken(string token)
        {
            var tokenHandler = new JsonWebTokenHandler();
            var key = _tokenKeyHandler.GetPublicKeys().First();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = T_ISSUER,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                tokenHandler.ValidateToken(token, validationParameters);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
