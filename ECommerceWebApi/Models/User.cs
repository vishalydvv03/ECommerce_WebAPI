﻿namespace ECommerceWebApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; } 
        public string Password { get; set; } 
        public bool IsDeleted { get; set; } = false;

        // Navigation property
        public ICollection<Order> Orders { get; set; }
    }
}
