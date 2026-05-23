using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Market.Domain.Interfaces.Externals;
using Market.Domain.Responses;
using Market.Domain.Settings;

namespace Market.Infrastructure.Repositories.Externals
{
    public class UserRepository : BaseClientRepository, IUserRepository
    {
        private readonly IClientAuthorizationRepository _clientAuthorizationRepository;
        private readonly ProjectSettings _options;
        public UserRepository(
            HttpClient httpClient,
            IClientAuthorizationRepository clientAuthorizationRepository,
            IOptions<ProjectSettings> options) : base(httpClient)
        {
            _options = options.Value;
            _httpClient.BaseAddress = new Uri(options.Value.AccountsApiUrl);
            _clientAuthorizationRepository = clientAuthorizationRepository;
        }

        public async Task<List<UserResponse>> GetUsersByIdsAsync(List<Guid> ids)
        {
            try
            {
                await AddToken();
                var userIdsQuery = string.Join("&", ids.Select(id => $"UserIds={id}"));
                var url = $"/v1/User?{userIdsQuery}";
                var resultado = await base.GetAsync<PaginatedResponse<UserResponse>>(url) ?? new PaginatedResponse<UserResponse>(new List<UserResponse>(), 0, 1, ids.Count, null);
                return resultado.Items;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching users: {ex.Message}");
                return new List<UserResponse>();
            }
        }

        protected async Task AddToken()
        {
            var token = await _clientAuthorizationRepository.AuthenticateAsync(_options.AccountsApiSlug);

            if (token != null)
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
        }
    }
}