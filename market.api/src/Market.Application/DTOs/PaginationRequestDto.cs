using Market.Domain.Enums;

namespace Market.Application.DTOs
{
    public class PaginationRequestDto
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public Guid? CategoryId { get; set; }
        public string? Search { get; set; }
        public ProductOrderEnum? Order { get; set; }
    }
}
