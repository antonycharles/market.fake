using User.Core.Requests;
using User.Core.Responses;

namespace User.Core.Handlers
{
    public interface IUserHandler
    {
        Task<PaginatedResponse<UserResponse>> GetAsync(PaginatedRequest request);
        Task<UserResponse> GetByIdAsync(Guid id);
        Task<UserResponse> CreateAsync(UserRequest request);
        Task UpdateAsync(Guid id, UserUpdateRequest request);
        Task DeleteAsync(Guid id);
    }
}
