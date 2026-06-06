namespace Market.Domain.Responses
{
    public class ProductListItemResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Code { get; set; }
        public string Slug { get; set; }
        public string Summary { get; set; }
        public Guid? ProductPriceId { get; set; }
        public decimal? OriginalPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public string? Currency { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public Guid? ProductPhotoId { get; set; }
        public string? PhotoFileId { get; set; }
        public string? PhotoUrl { get; set; }
        public string? PhotoDescription { get; set; }
        public long AvailableStock { get; set; }
        public long ReservedStock { get; set; }
        public long SoldStock { get; set; }
    }
}
