using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using User.Core.Handlers;

namespace User.Application.Handlers
{
    public class TokenKeyHandler : ITokenKeyHandler
    {
        private readonly IDistributedCache _cache;
        private const string KEY_CACHE = "accounts:token:key";

        public TokenKeyHandler(IDistributedCache cache)
        {
            _cache = cache;
        }

        public IList<JsonWebKey> GetPublicKeys()
        {
            var key = GetPrivateKey();
            var parameters = key.ECDsa.ExportParameters(true);

            return new List<JsonWebKey>
            {
                new()
                {
                    Kty = JsonWebAlgorithmsKeyTypes.EllipticCurve,
                    Use = "sig",
                    Kid = key.KeyId,
                    KeyId = key.KeyId,
                    X = Base64UrlEncoder.Encode(parameters.Q.X),
                    Y = Base64UrlEncoder.Encode(parameters.Q.Y),
                    Crv = JsonWebKeyECTypes.P256,
                    Alg = "ES256"
                }
            };
        }

        private ECDsaSecurityKey GetPrivateKey()
        {
            var jsonWebKey = GetJsonWebKey();
            var key = ECDsa.Create(new ECParameters
            {
                Curve = ECCurve.NamedCurves.nistP256,
                D = Base64UrlEncoder.DecodeBytes(jsonWebKey.D),
                Q = new ECPoint
                {
                    X = Base64UrlEncoder.DecodeBytes(jsonWebKey.X),
                    Y = Base64UrlEncoder.DecodeBytes(jsonWebKey.Y)
                }
            });

            return new ECDsaSecurityKey(key)
            {
                KeyId = jsonWebKey.KeyId
            };
        }

        private JsonWebKey GetJsonWebKey()
        {
            var key = _cache.GetString(KEY_CACHE);
            if (key != null)
                return JsonSerializer.Deserialize<JsonWebKey>(key);

            var jwk = GenerateJsonWebKey();
            _cache.SetString(KEY_CACHE, JsonSerializer.Serialize(jwk));
            return jwk;
        }

        private static JsonWebKey GenerateJsonWebKey()
        {
            var key = new ECDsaSecurityKey(ECDsa.Create(ECCurve.NamedCurves.nistP256))
            {
                KeyId = Guid.NewGuid().ToString()
            };

            var parameters = key.ECDsa.ExportParameters(true);
            return new JsonWebKey
            {
                Kty = JsonWebAlgorithmsKeyTypes.EllipticCurve,
                Use = "sig",
                Kid = key.KeyId,
                KeyId = key.KeyId,
                X = Base64UrlEncoder.Encode(parameters.Q.X),
                Y = Base64UrlEncoder.Encode(parameters.Q.Y),
                D = Base64UrlEncoder.Encode(parameters.D),
                Crv = JsonWebKeyECTypes.P256,
                Alg = "ES256"
            };
        }
    }
}
