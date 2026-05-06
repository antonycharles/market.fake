using User.Core.Entities;
using User.Core.Requests;
using User.Core.Responses;

namespace User.Application.Mappers
{
    public static class UserPhotoMap
    {
        public static UserPhoto ToUserPhoto(this UserPhotoRequest request)
        {
            return new UserPhoto
            {
                UserId = request.UserId,
                DocumentId = request.DocumentId,
                DocumentUrl = request.DocumentUrl
            };
        }

        public static void UpdateUserPhoto(this UserPhoto userPhoto, UserPhotoRequest request)
        {
            userPhoto.DocumentId = request.DocumentId;
            userPhoto.DocumentUrl = request.DocumentUrl;
            userPhoto.UpdatedAt = DateTime.UtcNow;
        }

        public static UserPhotoResponse ToUserPhotoResponse(this UserPhoto userPhoto) => new()
        {
            Id = userPhoto.Id,
            UserId = userPhoto.UserId,
            DocumentId = userPhoto.DocumentId,
            DocumentUrl = userPhoto.DocumentUrl
        };
    }
}
