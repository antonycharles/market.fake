using System.ComponentModel.DataAnnotations;

namespace User.Core.Entities
{
    public class UserAddress : BaseEntity
    {
        public Guid UserId { get; set; }
        public User? User { get; set; }

        [Required]
        [MaxLength(200)]
        public string Street { get; set; }

        [Required]
        [MaxLength(20)]
        public string Number { get; set; }

        [MaxLength(100)]
        public string? Complement { get; set; }

        [Required]
        [MaxLength(100)]
        public string Neighborhood { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; }

        [Required]
        [MaxLength(100)]
        public string State { get; set; }

        [Required]
        [MaxLength(20)]
        public string ZipCode { get; set; }

        [Required]
        [MaxLength(100)]
        public string Country { get; set; }

        public bool IsPrimary { get; set; }
    }
}
