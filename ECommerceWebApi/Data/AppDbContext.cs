using ECommerceWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceWebApi.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options): DbContext(options)
    {

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

    }
}
