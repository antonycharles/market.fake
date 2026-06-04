using System.ComponentModel.DataAnnotations;
using Market.Domain.Enums;

namespace Market.Application.DTOs
{
    public class ProductPriceDto : EntityDto
    {
        public Guid ProductId { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal SalePrice { get; set; }
        public string Currency { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class ProductPriceCreateDto
    {
        [Required]
        public Guid ProductId { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal SalePrice { get; set; }
        [Required]
        public string Currency { get; set; }
        [Required]
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class ProductPriceUpdateDto : ProductPriceCreateDto
    {
        public Guid Id { get; set; }
        [Required]
        public StatusEnum Status { get; set; }
    }
}
