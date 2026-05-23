using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accounts.Api.Helpers;
using Accounts.Core.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Accounts.Api.Controllers
{
    [ApiController]
    [Route("v{version:apiVersion}/.well-known")]
    [ApiVersion("1.0")]
    public class TokenController : ControllerBase
    {
        private readonly ILogger<TokenController> _logger;
        private readonly ITokenKeyHandler _tokenKeyHandler;

        public TokenController(
            ILogger<TokenController> logger,
            ITokenKeyHandler tokenKeyHandler)
        {
            _logger = logger;
            _tokenKeyHandler = tokenKeyHandler;
        }

        [HttpGet("jwks.json")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(JsonWebKey), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPublicKeys()
        {
            try
            {
                var result = _tokenKeyHandler.GetPublicKeys();

                return Ok(new
                {
                    Keys = result
                });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("openid-configuration")]
        [AllowAnonymous]
        public IActionResult GetOpenIdConfiguration()
        {
            var forwardedPrefix = Request.Headers["X-Forwarded-Prefix"].FirstOrDefault();
            var prefix = string.IsNullOrWhiteSpace(forwardedPrefix)
                ? Request.PathBase.Value
                : forwardedPrefix;

            prefix = string.IsNullOrWhiteSpace(prefix) ? string.Empty : $"/{prefix.Trim('/')}";
            var issuer = $"{Request.Scheme}://{Request.Host}{prefix}/v1";

            var config = new
            {
                issuer = issuer,
                authorization_endpoint = $"{issuer}/OAuth/authentication",
                token_endpoint = $"{issuer}/OAuth/token",
                userinfo_endpoint = $"{issuer}/OAuth/userinfo",
                jwks_uri = $"{issuer}/.well-known/jwks.json",
                response_types_supported = new[] { "code", "token", "id_token" },
                subject_types_supported = new[] { "public" },
                id_token_signing_alg_values_supported = new[] { "ES256" }
            };

            return Ok(config);
        }
    }
}
