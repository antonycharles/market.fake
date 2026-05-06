using User.Core.Enums;
using User.Core.Requests;
using User.Core.Responses;
using UserEntity = User.Core.Entities.User;

namespace User.Application.Mappers
{
    public static class UserMap
    {
        public static UserEntity ToUser(this UserRequest request) => new()
        {
            Name = request.Name,
            Email = request.Email,
            Status = request.Status ?? StatusEnum.Active
        };

        public static UserResponse ToUserResponse(this UserEntity user, string? fileUrl = "") => new()
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Status = user.Status,
            ImageUrl = user.UserPhoto != null ? fileUrl + "/File/" + user.UserPhoto.DocumentId : null
        };

        public static void Update(this UserEntity user, UserUpdateRequest request)
        {
            user.Name = request.Name;
            user.Email = request.Email;
            user.UpdatedAt = DateTime.UtcNow;
        }
    }
}
