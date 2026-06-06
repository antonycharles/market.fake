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
            var marketApiUrl = configuration["MarketApiUrl"]
                ?? throw new InvalidOperationException("MarketApiUrl setting is required.");

            _http = new HttpClient { BaseAddress = new Uri(marketApiUrl) };
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

        public async Task<List<CategoryDto>> GetAll(int pageSize = 100)
        {
            var response = await _http.GetFromJsonAsync<PaginatedResponse<CategoryDto>>(
                $"v1/Categories?PageIndex=1&PageSize={pageSize}");

            return response?.Items ?? new List<CategoryDto>();
        }
    }
}
