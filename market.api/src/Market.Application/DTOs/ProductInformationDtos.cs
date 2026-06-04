using System.ComponentModel.DataAnnotations;
using Market.Domain.Enums;

namespace Market.Application.DTOs
{
    public class ProductInformationDto : EntityDto
    {
        public Guid ProductId { get; set; }
        public InformationTypeEnum Type { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public int Order { get; set; }
    }

    public class ProductInformationCreateDto
    {
        [Required]
        public Guid ProductId { get; set; }
        [Required]
        public InformationTypeEnum Type { get; set; }
        [Required]
        public string Label { get; set; }
        [Required]
        public string Value { get; set; }
        public int Order { get; set; }
    }

    public class ProductInformationUpdateDto : ProductInformationCreateDto
    {
        public Guid Id { get; set; }
        [Required]
        public StatusEnum Status { get; set; }
    }
}
