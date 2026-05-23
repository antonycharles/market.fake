using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Market.Domain.Responses;

namespace Market.Domain.Interfaces.Externals
{
    public interface IClientAuthorizationRepository
    {
        Task<AuthenticationResponse> AuthenticateAsync(string apiSlug);
    }
}