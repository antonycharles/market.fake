using System.ComponentModel.DataAnnotations;

namespace User.Core.Entities
{
    public class UserCreditCard : BaseEntity
    {
        public Guid UserId { get; set; }
        public User? User { get; set; }

        [Required]
        [MaxLength(150)]
        public string HolderName { get; set; }

        [Required]
        [MaxLength(50)]
        public string Brand { get; set; }

        [Required]
        [MaxLength(4)]
        public string LastFourDigits { get; set; }

        [Range(1, 12)]
        public int ExpirationMonth { get; set; }

        [Range(2000, 9999)]
        public int ExpirationYear { get; set; }

        public bool IsPrimary { get; set; }
    }
}
