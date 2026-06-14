using System.ComponentModel.DataAnnotations;

namespace User.Core.Requests
{
    public class UserAddressRequest
    {
        public Guid? Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        public string Number { get; set; }

        public string? Complement { get; set; }

        [Required]
        public string Neighborhood { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string ZipCode { get; set; }

        [Required]
        public string Country { get; set; }

        public bool IsPrimary { get; set; }
    }
}
