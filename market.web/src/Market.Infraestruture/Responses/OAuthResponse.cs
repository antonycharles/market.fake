using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Market.Infraestruture.Responses
{
    public class OAuthResponse
    {
        public Guid AuthId { get; set; }
        public DateTime? ExpiresIn { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string? CallbackUrl { get; set; }
    }
}