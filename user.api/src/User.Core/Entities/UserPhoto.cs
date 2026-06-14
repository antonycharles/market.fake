using System.ComponentModel.DataAnnotations;
using User.Core.Enums;

namespace User.Core.Entities
{
    public class UserPhoto : BaseEntity
    {
        public Guid UserId { get; set; }
        public User? User { get; set; }

        [Required]
        public Guid DocumentId { get; set; }

        [Required]
        [MaxLength(200)]
        public string DocumentUrl { get; set; }

        [Required]
        public UserPhotoTypeEnum Type { get; set; } = UserPhotoTypeEnum.Secondary;
    }
}
