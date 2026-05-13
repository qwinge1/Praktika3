using Microsoft.AspNetCore.Mvc;
using AgroControl.API.Models;
using System.Linq;

namespace AgroControl.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            var user = _context.Users.FirstOrDefault(u => u.ИмяПользователя == dto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.ХэшПароля))
                return Unauthorized(new { success = false, message = "Неверный логин или пароль" });
            return Ok(new { success = true, message = "Вход выполнен" });
        }
    }

    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}