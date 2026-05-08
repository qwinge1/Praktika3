using Microsoft.AspNetCore.Mvc;
using AgroControl.API.Services;
using AgroControl.API.Models;

namespace AgroControl.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ReferenceService _refs;
        public UsersController(ReferenceService refs) => _refs = refs;

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(new { success = true, data = await _refs.GetUsersAsync() });

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _refs.GetUserAsync(id);
            return user == null
                ? NotFound(new { success = false, message = "Пользователь не найден" })
                : Ok(new { success = true, data = user });
        }

        [HttpPost]
        public IActionResult Create([FromBody] User user) =>
            Ok(new { success = true, message = "Пользователь создан (заглушка)" });

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] User user) =>
            Ok(new { success = true, message = "Пользователь обновлён (заглушка)" });

        [HttpDelete("{id}")]
        public IActionResult Delete(int id) =>
            Ok(new { success = true, message = "Пользователь удалён (заглушка)" });
    }
}