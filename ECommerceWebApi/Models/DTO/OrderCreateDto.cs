namespace ECommerceWebApi.Models.DTO
{
    public class OrderCreateDto
    {
        public int UserId { get; set; }
        public ICollection<OrderItemCreateDto> OrderItems { get; set; }
    }
}
