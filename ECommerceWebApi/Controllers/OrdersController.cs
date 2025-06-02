using ECommerceWebApi.Data;
using ECommerceWebApi.Models.DTO;
using ECommerceWebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(AppDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderReadDto>>> GetOrders()
        {
            var orders = await context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Select(o => new OrderReadDto
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    UserId = o.UserId,
                    UserEmail = o.User.Email,
                    OrderItems = o.OrderItems.Select(oi => new OrderItemReadDto
                    {
                        Id = oi.Id,
                        ProductId = oi.ProductId,
                        ProductName = oi.Product.Name,
                        Quantity = oi.Quantity,
                        ProductPrice = oi.Product.Price
                    }).ToList()
                })
                .ToListAsync();

            return Ok(orders);
        }

        // GET: api/orders/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderReadDto>> GetOrder(int id)
        {
            var order = await context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            var dto = new OrderReadDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                UserId = order.UserId,
                UserEmail = order.User.Email,
                OrderItems = order.OrderItems.Select(oi => new OrderItemReadDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    Quantity = oi.Quantity,
                    ProductPrice = oi.Product.Price
                }).ToList()
            };

            return Ok(dto);
        }

        // POST: api/orders
        [HttpPost]
        public async Task<ActionResult<OrderReadDto>> CreateOrder(OrderCreateDto dto)
        {
            // Check if user exists
            var user = await context.Users.FindAsync(dto.UserId);
            if (user == null) return BadRequest("User does not exist.");

            // Validate all products in order items
            var productIds = dto.OrderItems.Select(oi => oi.ProductId).Distinct();
            var products = await context.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();
            if (products.Count != productIds.Count())
            {
                return BadRequest("One or more products do not exist.");
            }

            var order = new Order
            {
                UserId = dto.UserId,
                OrderDate = DateTime.Now,
                OrderItems = dto.OrderItems.Select(oi => new OrderItem
                {
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity
                }).ToList()
            };

            context.Orders.Add(order);
            await context.SaveChangesAsync();

            // Prepare return DTO
            var resultDto = new OrderReadDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                UserId = user.Id,
                UserEmail = user.Email,
                OrderItems = order.OrderItems.Select(oi =>
                {
                    var product = products.First(p => p.Id == oi.ProductId);
                    return new OrderItemReadDto
                    {
                        Id = oi.Id,
                        ProductId = oi.ProductId,
                        ProductName = product.Name,
                        Quantity = oi.Quantity,
                        ProductPrice = product.Price
                    };
                }).ToList()
            };

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, resultDto);
        }

        // PUT: api/orders/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, OrderCreateDto dto)
        {
            var order = await context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            // Validate user
            if (order.UserId != dto.UserId)
                return BadRequest("UserId mismatch.");

            // Validate products
            var productIds = dto.OrderItems.Select(oi => oi.ProductId).Distinct();
            var products = await context.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();
            if (products.Count != productIds.Count())
            {
                return BadRequest("One or more products do not exist.");
            }

            // Remove old order items
            context.OrderItems.RemoveRange(order.OrderItems);

            // Add new order items from DTO
            order.OrderItems = dto.OrderItems.Select(oi => new OrderItem
            {
                ProductId = oi.ProductId,
                Quantity = oi.Quantity
            }).ToList();

            // Update order date to now (optional)
            order.OrderDate = DateTime.Now;

            await context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/orders/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            // Remove related order items first
            context.OrderItems.RemoveRange(order.OrderItems);

            // Remove order
            context.Orders.Remove(order);

            await context.SaveChangesAsync();

            return NoContent();
        }

    }
}
