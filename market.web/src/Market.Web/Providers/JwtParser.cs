using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Market.Web.Providers
{
    public static class JwtParser
    {
        public static ClaimsIdentity ParseClaimsFromJwt(string token)
        {
            var payload = token.Split('.')[1];
            var json = ParseBase64WithoutPadding(payload);
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            var claims = new List<Claim>();

            claims.Add(new Claim("access_token", token));
            foreach (var item in data)
            {
                if(item.Value is JsonElement element && element.ValueKind == JsonValueKind.Array)
                {
                    foreach (var arrayItem in element.EnumerateArray())
                    {
                        claims.Add(new Claim(item.Key, arrayItem.ToString()!));
                    }
                }
                else
                {
                    claims.Add(new Claim(item.Key, item.Value.ToString()!));
                }
            }

            return new ClaimsIdentity(claims, "jwt");
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            return Convert.FromBase64String(base64);
        }
    }
}