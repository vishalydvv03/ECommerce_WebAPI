﻿namespace ECommerceWebApi.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        // Foreign keys and navigation
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int Quantity { get; set; } //Reference Navigation
    }
}
