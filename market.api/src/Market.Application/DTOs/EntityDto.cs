using Market.Domain.Enums;

namespace Market.Application.DTOs
{
    public class EntityDto
    {
        public Guid Id { get; set; }
        public StatusEnum Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
