using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Project.Web.DTOs;
using Project.Web.Exceptions;

namespace Project.Web.Services
{
    public class ProjectService
    {
        private readonly HttpClient _http;
        private readonly AuthenticationStateProvider _authStateProvider;

        public ProjectService(IConfiguration configuration, AuthenticationStateProvider authStateProvider)
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
                // Supondo que o token está salvo como claim "access_token"
                var token = user.FindFirst("access_token")?.Value;
                if (!string.IsNullOrEmpty(token))
                {
                    _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
        }

        public async Task<List<ProjectDto>> GetAll()
        {
            await AddBearerTokenAsync();
            return await _http.GetFromJsonAsync<List<ProjectDto>>("v1/Project");
        }

        public async Task<ProjectDto> Get(Guid id)
        {
            await AddBearerTokenAsync();
            return await _http.GetFromJsonAsync<ProjectDto>($"v1/Project/{id}");
        }

        public async Task<ProjectDto> Create(ProjectCreateDto dto)
        {
            await AddBearerTokenAsync();
            var res = await _http.PostAsJsonAsync("v1/Project", dto);
            if (res.IsSuccessStatusCode)
            {
                return await res.Content.ReadFromJsonAsync<ProjectDto>();
            }
            else
            {
                var content = await res.Content.ReadAsStringAsync();
                try
                {
                    using var doc = System.Text.Json.JsonDocument.Parse(content);

                    if (doc.RootElement.TryGetProperty("errors", out var errorsProp))
                    {
                        var messages = new List<string>();
                        foreach (var prop in errorsProp.EnumerateObject())
                        {
                            foreach (var err in prop.Value.EnumerateArray())
                            {
                                Console.WriteLine(err.GetString());
                                messages.Add(err.ToString());
                            }
                        }
                        if (messages.Count > 0)
                        {
                            throw new NegocioException(string.Join("; ", messages));
                        }
                    }
                }
                catch(NegocioException ex) { throw ex; }
                
                throw new Exception($"API error: {content}");
            }
        }

        public async Task<ProjectDto> Update(ProjectDto dto)
        {
            await AddBearerTokenAsync();
            var res = await _http.PutAsJsonAsync($"v1/Project/{dto.Id}", dto);
            if (res.IsSuccessStatusCode)
            {
                if (res.StatusCode == System.Net.HttpStatusCode.NoContent)
                    return dto;

                return await res.Content.ReadFromJsonAsync<ProjectDto>();
            }
            else
            {
                var content = await res.Content.ReadAsStringAsync();
                try
                {
                    using var doc = System.Text.Json.JsonDocument.Parse(content);
                    if (doc.RootElement.TryGetProperty("errors", out var errorsProp))
                    {
                        var messages = new List<string>();
                        foreach (var prop in errorsProp.EnumerateObject())
                        {
                            foreach (var err in prop.Value.EnumerateArray())
                            {
                                messages.Add(err.GetString());
                            }
                        }
                        if (messages.Count > 0)
                        {
                            throw new NegocioException(string.Join("; ", messages));
                        }
                    }
                }
                catch(NegocioException ex) { throw ex; }
                throw new Exception($"API error: {content}");
            }
        }

        public async Task Delete(Guid id)
        {
            await AddBearerTokenAsync();
            var res = await _http.DeleteAsync($"v1/Project/{id}");

            if (!res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync();
                throw new Exception($"API error: {content}");
            }
        }
    }
}
