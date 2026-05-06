using System.ComponentModel.DataAnnotations;

namespace User.Core.Entities
{
    public class User : BaseEntity
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        [Required]
        [MaxLength(150)]
        public string Email { get; set; }

        [Required]
        [MaxLength(200)]
        public string Password { get; set; }

        public UserPhoto? UserPhoto { get; set; }
    }
}
