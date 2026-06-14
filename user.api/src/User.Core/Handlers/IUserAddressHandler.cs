using User.Core.Requests;
using User.Core.Responses;

namespace User.Core.Handlers
{
    public interface IUserAddressHandler
    {
        Task<List<UserAddressResponse>> GetByUserIdAsync(Guid userId);
        Task<UserAddressResponse> GetByIdAsync(Guid id);
        Task UpdateOrCreateAsync(UserAddressRequest request);
        Task DeleteAsync(Guid id);
        Task DeleteMeAsync(Guid id, Guid userId);
    }
}
