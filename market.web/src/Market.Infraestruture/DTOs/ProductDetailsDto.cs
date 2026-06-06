namespace Market.Infraestruture.DTOs
{
    public class ProductDetailsDto
    {
        public Guid Id { get; set; }
        public Guid StoreId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Code { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public string? Description { get; set; }
    }
}
