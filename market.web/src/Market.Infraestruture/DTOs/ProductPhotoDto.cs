namespace Market.Infraestruture.DTOs
{
    public class ProductPhotoDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string FileId { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Order { get; set; }
        public int Type { get; set; }
    }
}
