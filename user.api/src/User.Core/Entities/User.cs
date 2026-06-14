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

        public ICollection<UserPhoto> UserPhotos { get; set; } = [];
        public ICollection<UserAddress> UserAddresses { get; set; } = [];
        public ICollection<UserCreditCard> UserCreditCards { get; set; } = [];
    }
}
