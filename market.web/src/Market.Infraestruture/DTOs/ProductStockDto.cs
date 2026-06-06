namespace Market.Infraestruture.DTOs
{
    public class ProductStockDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public long AvailableStock { get; set; }
        public long ReservedStock { get; set; }
        public long SoldStock { get; set; }
    }
}
