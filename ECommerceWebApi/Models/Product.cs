namespace ECommerceWebApi.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public decimal Price { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Foreign key and navigation
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        // Navigation property
        public ICollection<OrderItem> OrderItems { get; set; } 
    }
}
