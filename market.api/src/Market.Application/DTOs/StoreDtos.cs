using System.ComponentModel.DataAnnotations;
using Market.Domain.Enums;

namespace Market.Application.DTOs
{
    public class StoreDto : EntityDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid UserCreatedId { get; set; }
    }

    public class StoreCreateDto
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid UserCreatedId { get; set; }
    }

    public class StoreUpdateDto : StoreCreateDto
    {
        public Guid Id { get; set; }
        [Required]
        public StatusEnum Status { get; set; }
    }
}
