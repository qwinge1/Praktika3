using Microsoft.AspNetCore.Mvc;

using AgroControl.API.Services;



namespace AgroControl.API.Controllers

{

    [ApiController]

    [Route("api/[controller]")]

    public class AuthController : ControllerBase

    {

        private readonly AuthService _authService;

        public AuthController(AuthService authService) => _authService = authService;



        [HttpPost("login")]

        public IActionResult Login([FromBody] LoginDto dto)

        {

            var token = _authService.Login(dto.Username, dto.Password);

            if (token == null)

                return Unauthorized(new { success = false, message = "Неверный логин или пароль" });

            return Ok(new { success = true, token });

        }



        [HttpPost("register")]

        public IActionResult Register([FromBody] RegisterDto dto)

        {

            var ok = _authService.Register(dto.Username, dto.Password, dto.FullName, dto.Role, dto.Email, dto.Department);

            if (!ok) return BadRequest("Пользователь уже существует");

            return Ok(new { success = true });

        }

    }



    public class LoginDto

    {

        public string Username { get; set; } = "";

        public string Password { get; set; } = "";

    }



    public class RegisterDto

    {

        public string Username { get; set; } = "";

        public string Password { get; set; } = "";

        public string FullName { get; set; } = "";

        public string Role { get; set; } = "operator";

        public string Email { get; set; } = "";

        public string Department { get; set; } = "";

    }

}