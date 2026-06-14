using User.Core.Requests;
using User.Core.Responses;

namespace User.Core.Handlers
{
    public interface IUserPhotoHandler
    {
        Task<List<UserPhotoResponse>> GetByUserIdAsync(Guid userId);
        Task<UserPhotoResponse> GetByIdAsync(Guid id);
        Task UpdateOrCreateAsync(UserPhotoRequest request);
        Task DeleteAsync(Guid id);
        Task DeleteMeAsync(Guid id, Guid userId);
    }
}
