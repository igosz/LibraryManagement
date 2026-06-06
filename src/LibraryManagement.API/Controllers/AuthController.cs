using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.API.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Infrastructure.Data;

namespace LibraryManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public AuthController(LibraryDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            // sprawdzenie emaila
            var emailExists = await _context.Users
                .AnyAsync(u => u.Email == registerDto.Email);

            if (emailExists)
            {
                return BadRequest("Email already exists");
            }

            // sprawdzenie username
            var usernameExists = await _context.Users
                .AnyAsync(u => u.Username == registerDto.Username);

            if (usernameExists)
            {
                return BadRequest("Username already exists");
            }

            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,

                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password)
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "User registered successfully"
            });
        }
    }
}