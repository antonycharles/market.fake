using System.ComponentModel.DataAnnotations;
using Market.Domain.Enums;

namespace Market.Application.DTOs
{
    public class ProductDto : EntityDto
    {
        public Guid StoreId { get; set; }
        public string Name { get; set; }
        public int Code { get; set; }
        public string Slug { get; set; }
        public string? Summary { get; set; }
        public string? Description { get; set; }
    }

    public class ProductCreateDto
    {
        [Required]
        public Guid StoreId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Code { get; set; }
        [Required]
        public string Slug { get; set; }
        public string? Summary { get; set; }
        public string? Description { get; set; }
    }

    public class ProductUpdateDto : ProductCreateDto
    {
        public Guid Id { get; set; }
        [Required]
        public StatusEnum Status { get; set; }
    }

    public class ProductListItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Code { get; set; }
        public string Slug { get; set; }
        public string? Summary { get; set; }
        public ProductPriceDto? ProductPrice { get; set; }
        public ProductPhotoDto? ProductPhoto { get; set; }
    }
}
