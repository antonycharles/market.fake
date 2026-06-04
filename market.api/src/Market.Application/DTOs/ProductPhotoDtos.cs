using System.ComponentModel.DataAnnotations;
using Market.Domain.Enums;

namespace Market.Application.DTOs
{
    public class ProductPhotoDto : EntityDto
    {
        public Guid ProductId { get; set; }
        public string FileId { get; set; }
        public string Url { get; set; }
        public string? Description { get; set; }
        public int Order { get; set; }
        public ProductPhotoEnum Type { get; set; }
    }

    public class ProductPhotoCreateDto
    {
        [Required]
        public Guid ProductId { get; set; }
        [Required]
        public string FileId { get; set; }
        [Required]
        public string Url { get; set; }
        public string? Description { get; set; }
        public int Order { get; set; }
        [Required]
        public ProductPhotoEnum Type { get; set; }
    }

    public class ProductPhotoUpdateDto : ProductPhotoCreateDto
    {
        public Guid Id { get; set; }
        [Required]
        public StatusEnum Status { get; set; }
    }
}
