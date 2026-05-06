using System.ComponentModel.DataAnnotations;
using User.Core.Enums;

namespace User.Core.Entities
{
    public class BaseEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public StatusEnum Status { get; set; } = StatusEnum.Active;
        public bool IsDeleted { get; set; } = false;
    }
}
