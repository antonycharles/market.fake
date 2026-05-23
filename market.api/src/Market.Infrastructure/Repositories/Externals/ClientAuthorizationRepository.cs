using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Market.Domain.Interfaces.Externals;
using Market.Domain.Responses;
using Market.Domain.Settings;

namespace Market.Infrastructure.Repositories.Externals
{
    public class ClientAuthorizationRepository : BaseClientRepository, IClientAuthorizationRepository
    {
        private static AuthenticationResponse? authenticationResponse;
        private readonly ProjectSettings _options;
        
        public ClientAuthorizationRepository(HttpClient httpClient, IOptions<ProjectSettings> options) : base(httpClient)
        {
            _options = options.Value;
            _httpClient.BaseAddress = new Uri(options.Value.AccountsApiUrl);
        }

        public async Task<AuthenticationResponse> AuthenticateAsync(string apiSlug)
        {
            if(authenticationResponse != null && authenticationResponse.ExpiresIn > DateTime.Now)
                return authenticationResponse;

            authenticationResponse = await base.PostFormDataAsync<AuthenticationResponse>("OAuth/token", new Dictionary<string, string>
            {
                { "ClientId", _options.ClientId },
                { "AppSlug", _options.AccountsApiSlug },
                { "ClientSecret", _options.ClientSecret },
                { "GrantType", "client_credentials" }
            });

            return authenticationResponse;
        }
    }
}