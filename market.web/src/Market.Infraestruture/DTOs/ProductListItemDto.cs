namespace Market.Infraestruture.DTOs
{
    public class ProductListItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Code { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public ProductPriceDto? ProductPrice { get; set; }
        public ProductPhotoDto? ProductPhoto { get; set; }
    }
}
