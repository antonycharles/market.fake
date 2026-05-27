using System.Net.Http.Headers;
using System.Net.Http.Json;
using Market.Infraestruture.DTOs;
using Microsoft.AspNetCore.Components.Authorization;

namespace Market.Infraestruture.Services
{
    public class CategoryService
    {
        private readonly HttpClient _http;
        private readonly AuthenticationStateProvider _authStateProvider;

        public CategoryService(IConfiguration configuration, AuthenticationStateProvider authStateProvider)
        {
            _http = new HttpClient { BaseAddress = new Uri(configuration["MarketApiUrl"]) };
            _authStateProvider = authStateProvider;
        }

        private async Task AddBearerTokenAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                var token = user.FindFirst("access_token")?.Value;
                if (!string.IsNullOrEmpty(token))
                {
                    _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
        }

        public async Task<List<CategoryDto>> GetAll()
        {
            await AddBearerTokenAsync();
            return await _http.GetFromJsonAsync<List<CategoryDto>>("v1/Category") ?? new List<CategoryDto>();
        }
    }
}
