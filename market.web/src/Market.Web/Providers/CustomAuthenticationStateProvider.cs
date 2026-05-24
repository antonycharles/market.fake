using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Market.Web.Responses;

namespace Market.Web.Providers
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;

        public CustomAuthenticationStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<OAuthResponse>("jwt");

            ClaimsIdentity identity;

            if (token == null || string.IsNullOrEmpty(token.Token) || token.ExpiresIn < DateTime.Now)
            {
                await _localStorage.RemoveItemAsync("jwt");
                identity = new ClaimsIdentity();
            }
            else
            {
                identity = JwtParser.ParseClaimsFromJwt(token.Token);
            }

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public void NotifyUserAuthentication(string token)
        {
            var identity = JwtParser.ParseClaimsFromJwt(token);
            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public void NotifyUserLogout()
        {
            var anon = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anon)));
        }
    }
}