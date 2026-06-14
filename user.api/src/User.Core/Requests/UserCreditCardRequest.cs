using System.ComponentModel.DataAnnotations;

namespace User.Core.Requests
{
    public class UserCreditCardRequest
    {
        public Guid? Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string HolderName { get; set; }

        [Required]
        public string Brand { get; set; }

        [Required]
        [MaxLength(4)]
        [MinLength(4)]
        public string LastFourDigits { get; set; }

        [Range(1, 12)]
        public int ExpirationMonth { get; set; }

        [Range(2000, 9999)]
        public int ExpirationYear { get; set; }

        public bool IsPrimary { get; set; }
    }
}
