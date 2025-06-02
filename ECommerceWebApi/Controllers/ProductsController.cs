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
    public class ProductsController(AppDbContext context) : ControllerBase
    {
        // GET: api/product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductReadDto>>> GetProducts()
        {
            var products = await context.Products
                .Include(p => p.Category)
                .Select(p => new ProductReadDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name
                })
                .ToListAsync();

            return Ok(products);
        }

        // GET: api/product/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductReadDto>> GetProduct(int id)
        {
            var product = await context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            var dto = new ProductReadDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name
            };

            return Ok(dto);
        }

        // POST: api/product
        [HttpPost]
        public async Task<ActionResult<ProductReadDto>> CreateProduct(ProductCreateDto dto)
        {
            var categoryExists = await context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
            if (!categoryExists)
            {
                return BadRequest("Category does not exist.");
            }

            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                CategoryId = dto.CategoryId
            };

            context.Products.Add(product);
            await context.SaveChangesAsync();

            var resultDto = new ProductReadDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                CategoryId = product.CategoryId,
                CategoryName = (await context.Categories.FindAsync(product.CategoryId))!.Name
            };

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, resultDto);
        }

        // PUT: api/product/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, ProductReadDto dto)
        {
            var product = await context.Products.FindAsync(id);
            if (product == null) return NotFound();

            var categoryExists = await context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
            if (!categoryExists)
            {
                return BadRequest("Category does not exist.");
            }

            product.Name = dto.Name;
            product.Price = dto.Price;
            product.CategoryId = dto.CategoryId;

            await context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/product/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await context.Products.FindAsync(id);
            if (product == null) return NotFound();

            context.Products.Remove(product);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
