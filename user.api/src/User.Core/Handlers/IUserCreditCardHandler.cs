using User.Core.Requests;
using User.Core.Responses;

namespace User.Core.Handlers
{
    public interface IUserCreditCardHandler
    {
        Task<List<UserCreditCardResponse>> GetByUserIdAsync(Guid userId);
        Task<UserCreditCardResponse> GetByIdAsync(Guid id);
        Task<UserCreditCardResponse> GetByIdMeAsync(Guid id, Guid userId);
        Task UpdateOrCreateAsync(UserCreditCardRequest request);
        Task DeleteAsync(Guid id);
        Task DeleteMeAsync(Guid id, Guid userId);
    }
}
