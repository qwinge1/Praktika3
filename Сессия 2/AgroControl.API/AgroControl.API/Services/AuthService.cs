using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using AgroControl.API.Models;

namespace AgroControl.API.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly string _jwtSecret;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _jwtSecret = config["Jwt:Key"]!;
        }

        public string? Login(string username, string password)
        {
            var user = _context.Users.SingleOrDefault(u => u.ИмяПользователя == username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.ХэшПароля))
                return null;

            user.ПоследнийВход = DateTime.UtcNow;
            _context.SaveChanges();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("userId", user.ID.ToString()),
                    new Claim("username", user.ИмяПользователя),
                    new Claim(ClaimTypes.Role, user.Роль),
                    new Claim("fullName", user.ПолноеИмя),
                    new Claim("department", user.Отдел ?? "")
                }),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = "AgroControl",
                Audience = "AgroControl"
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public bool Register(string username, string password, string fullName, string role, string email, string department)
        {
            if (_context.Users.Any(u => u.ИмяПользователя == username))
                return false;

            _context.Users.Add(new User
            {
                ИмяПользователя = username,
                ХэшПароля = BCrypt.Net.BCrypt.HashPassword(password),
                ПолноеИмя = fullName,
                Роль = role,
                Email = email,
                Отдел = department,
                Активен = true,
                ДатаСоздания = DateTime.UtcNow
            });
            _context.SaveChanges();
            return true;
        }
    }
}