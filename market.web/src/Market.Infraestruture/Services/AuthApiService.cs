using System.Net.Http.Json;
using Market.Infraestruture.Responses;

namespace Market.Infraestruture.Services
{
    public class LoginWebService
    {
        private readonly HttpClient _httpClient;

        public LoginWebService(IConfiguration configuration)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(configuration["LoginWebUrl"]) };
        }

        public async Task<OAuthResponse> ExchangeCodeForJwtAsync(string code)
        {
            var url = $"Login/token?code={code}";

            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<OAuthResponse>();
            return json ?? new OAuthResponse();
        }

    }
}