using System.ComponentModel.DataAnnotations;
using Market.Domain.Enums;

namespace Market.Application.DTOs
{
    public class ProductCategoryDto : EntityDto
    {
        public Guid ProductId { get; set; }
        public Guid CategoryId { get; set; }
        public int Order { get; set; }
    }

    public class ProductCategoryCreateDto
    {
        [Required]
        public Guid ProductId { get; set; }
        [Required]
        public Guid CategoryId { get; set; }
        public int Order { get; set; }
    }

    public class ProductCategoryUpdateDto : ProductCategoryCreateDto
    {
        public Guid Id { get; set; }
        [Required]
        public StatusEnum Status { get; set; }
    }
}
