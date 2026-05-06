using System.ComponentModel.DataAnnotations;

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
    }
}
