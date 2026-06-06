namespace Market.Infraestruture.DTOs
{
    public class ProductInformationDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int Type { get; set; }
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public int Order { get; set; }
    }
}
