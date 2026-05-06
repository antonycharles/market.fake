using System.ComponentModel.DataAnnotations;

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
    }
}
