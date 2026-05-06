using System.ComponentModel.DataAnnotations;
using User.Core.Enums;

namespace User.Core.Requests
{
    public class UserRequest
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        public StatusEnum? Status { get; set; }
    }
}
