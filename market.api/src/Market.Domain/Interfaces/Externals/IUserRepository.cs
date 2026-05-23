using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Market.Domain.Responses;

namespace Market.Domain.Interfaces.Externals
{
    public interface IUserRepository
    {
        Task<List<UserResponse>> GetUsersByIdsAsync(List<Guid> ids);
    }
}