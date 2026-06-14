using System.ComponentModel.DataAnnotations;
using User.Core.Enums;

namespace User.Core.Requests
{
    public class UserPhotoRequest
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid DocumentId { get; set; }

        [Required]
        public string DocumentUrl { get; set; }

        public UserPhotoTypeEnum Type { get; set; } = UserPhotoTypeEnum.Secondary;
    }
}
