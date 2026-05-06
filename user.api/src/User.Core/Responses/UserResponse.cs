using User.Core.Enums;

namespace User.Core.Responses
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public StatusEnum Status { get; set; }
        public string? ImageUrl { get; set; }
    }
}
