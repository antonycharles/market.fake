using System.ComponentModel.DataAnnotations;
using Market.Domain.Enums;

namespace Market.Application.DTOs
{
    public class ProductStockDto : EntityDto
    {
        public Guid ProductId { get; set; }
        public long AvailableStock { get; set; }
        public long ReservedStock { get; set; }
        public long SoldStock { get; set; }
    }

    public class ProductStockCreateDto
    {
        [Required]
        public Guid ProductId { get; set; }
        public long AvailableStock { get; set; }
        public long ReservedStock { get; set; }
        public long SoldStock { get; set; }
    }

    public class ProductStockUpdateDto : ProductStockCreateDto
    {
        public Guid Id { get; set; }
        [Required]
        public StatusEnum Status { get; set; }
    }
}
