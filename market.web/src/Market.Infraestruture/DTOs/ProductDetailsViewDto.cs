namespace Market.Infraestruture.DTOs
{
    public class ProductDetailsViewDto
    {
        public ProductDto Product { get; set; } = new();
        public ProductPriceDto? Price { get; set; }
        public List<ProductPhotoDto> Photos { get; set; } = new();
        public List<ProductInformationDto> Informations { get; set; } = new();
        public ProductStockDto? Stock { get; set; }
    }
}
