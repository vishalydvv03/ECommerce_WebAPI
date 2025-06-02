namespace ECommerceWebApi.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        // Foreign key and navigation
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Navigation property
        public ICollection<OrderItem> OrderItems { get; set; } 
    }
}
