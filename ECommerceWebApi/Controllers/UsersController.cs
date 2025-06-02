using ECommerceWebApi.Data;
using ECommerceWebApi.Models.DTO;
using ECommerceWebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly PasswordHasher<User> hasher = new();

        public UsersController(AppDbContext context)
        {
            this.context = context;
        }

        // POST: api/users/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            if (await context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Email already exists.");

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = hasher.HashPassword(null!, dto.Password)
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return Ok(new UserReadDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            });
        }

        // POST: api/users/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email && !u.IsDeleted);

            if (user == null)
                return Unauthorized("Invalid email or password.");

            var result = hasher.VerifyHashedPassword(user, user.Password, dto.Password);
            if (result == PasswordVerificationResult.Success)
            {
                return Ok(new UserReadDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email
                });
            }

            return Unauthorized("Invalid email or password.");
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UserUpdateDto dto)
        {
            var user = await context.Users.FindAsync(id);
            if (user == null || user.IsDeleted)
                return NotFound();

            user.Name = dto.Name;
            user.Email = dto.Email;

            if (!string.IsNullOrEmpty(dto.Password))
            {
                user.Password = hasher.HashPassword(user, dto.Password);
            }

            await context.SaveChangesAsync();
            return Ok("User updated successfully.");
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var user = await context.Users.FindAsync(id);
            if (user == null || user.IsDeleted)
                return NotFound();

            user.IsDeleted = true;
            await context.SaveChangesAsync();

            return Ok("User soft deleted.");
        }
    }
}