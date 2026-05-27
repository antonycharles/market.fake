using Blazored.LocalStorage;
using Market.Infraestruture.Responses;

namespace Market.Infraestruture.Services
{
    public class ApiAuthHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;

        public ApiAuthHandler(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _localStorage.GetItemAsync<OAuthResponse>("jwt");

            if (token != null && !string.IsNullOrEmpty(token.Token))
            {
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}