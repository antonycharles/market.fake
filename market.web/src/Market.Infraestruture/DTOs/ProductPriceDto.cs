namespace Market.Infraestruture.DTOs
{
    public class ProductPriceDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal SalePrice { get; set; }
        public string Currency { get; set; } = "USD";
        public DateTime ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }
}
