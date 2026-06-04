using System.ComponentModel.DataAnnotations;
using Market.Domain.Enums;

namespace Market.Application.DTOs
{
    public class CategoryDto : EntityDto
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public string? Description { get; set; }
    }

    public class CategoryCreateDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Slug { get; set; }
        public string? Description { get; set; }
    }

    public class CategoryUpdateDto : CategoryCreateDto
    {
        public Guid Id { get; set; }
        [Required]
        public StatusEnum Status { get; set; }
    }
}
