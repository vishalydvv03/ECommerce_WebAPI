namespace ECommerceWebApi.Models.DTO
{
    public class OrderReadDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public ICollection<OrderItemReadDto> OrderItems { get; set; } 
    }
}
