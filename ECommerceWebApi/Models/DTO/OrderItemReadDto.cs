namespace ECommerceWebApi.Models.DTO
{
    public class OrderItemReadDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal ProductPrice { get; set; }
    }
}
