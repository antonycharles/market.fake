using System.ComponentModel.DataAnnotations;

namespace User.Core.Requests
{
    public class UserUpdateRequest
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        public string? Password { get; set; }
    }
}
