using ECommerceWebApi.Models.DTO;
using ECommerceWebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ECommerceWebApi.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerceWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(AppDbContext context) : ControllerBase
    {

        // GET: api/category
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryReadDto>>> GetCategories()
        {
            var categories = await context.Categories
                .Select(c => new CategoryReadDto { Id = c.Id, Name = c.Name })
                .ToListAsync();

            return Ok(categories);
        }

        // GET: api/category/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryReadDto>> GetCategory(int id)
        {
            var category = await context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            var dto = new CategoryReadDto { Id = category.Id, Name = category.Name };
            return Ok(dto);
        }

        // POST: api/category
        [HttpPost]
        public async Task<ActionResult<CategoryReadDto>> CreateCategory(CategoryDto dto)
        {
            var category = new Category { Name = dto.Name };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var resultDto = new CategoryReadDto { Id = category.Id, Name = category.Name };
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, resultDto);
        }

        // PUT: api/category/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryDto dto)
        {
            var category = await context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            category.Name = dto.Name;
            await context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/category/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            context.Categories.Remove(category);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
