using User.Core.Requests;
using User.Core.Responses;

namespace User.Core.Handlers
{
    public interface IUserPhotoHandler
    {
        Task<UserPhotoResponse> GetByIdAsync(Guid id);
        Task UpdateOrCreateAsync(UserPhotoRequest request);
        Task DeleteAsync(Guid id);
    }
}
