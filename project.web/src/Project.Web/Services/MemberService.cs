using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Project.Web.DTOs;

namespace Project.Web.Services
{
    public class MemberService
    {
        private readonly HttpClient _http;
        private readonly AuthenticationStateProvider _authStateProvider;

        public MemberService(IConfiguration configuration, AuthenticationStateProvider authStateProvider)
        {
            _http = new HttpClient { BaseAddress = new Uri(configuration["AccountsApiUrl"]) };
            _authStateProvider = authStateProvider;
        }

        private async Task AddBearerTokenAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            if (user.Identity.IsAuthenticated)
            {
                var token = user.FindFirst("access_token")?.Value;
                if (!string.IsNullOrEmpty(token))
                {
                    _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
            }
        }

        public async Task<MemberDto> GetByIdAsync(Guid id)
        {
            await AddBearerTokenAsync();
            var response = await _http.GetAsync($"v1/Member/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<MemberDto>();
            }
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            throw new Exception($"Error getting member: {response.ReasonPhrase}");
        }

        public async Task<List<MemberDto>> GetAllAsync(Guid? projectId = null)
        {
            await AddBearerTokenAsync();
            string url = "v1/Member";
            if (projectId.HasValue)
                url += $"?projectId={projectId}";
            return await _http.GetFromJsonAsync<List<MemberDto>>(url);
        }

        public async Task<MemberDto> CreateAsync(MemberCreateDto dto)
        {
            await AddBearerTokenAsync();
            var response = await _http.PostAsJsonAsync("v1/Member", dto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<MemberDto>();
            }
            var problem = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error creating member: {problem}");
        }

        public async Task DeleteAsync(Guid id)
        {
            await AddBearerTokenAsync();
            var response = await _http.DeleteAsync($"v1/Member/{id}");
            if (!response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                var problem = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error deleting member: {problem}");
            }
        }
    }
}
