using Microsoft.AspNetCore.Mvc;
using AgroControl.API.Services;
using AgroControl.API.Models;

namespace AgroControl.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EquipmentController : ControllerBase
    {
        private readonly ReferenceService _refs;
        public EquipmentController(ReferenceService refs) => _refs = refs;

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(new { success = true, data = await _refs.GetEquipmentAsync() });

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var eq = await _refs.GetEquipmentAsync(id);
            return eq == null
                ? NotFound(new { success = false, message = "Оборудование не найдено" })
                : Ok(new { success = true, data = eq });
        }

        [HttpPost]
        public IActionResult Create([FromBody] Equipment equipment) =>
            Ok(new { success = true, message = "Оборудование создано (заглушка)" });

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Equipment equipment) =>
            Ok(new { success = true, message = "Оборудование обновлено (заглушка)" });

        [HttpDelete("{id}")]
        public IActionResult Delete(int id) =>
            Ok(new { success = true, message = "Оборудование удалено (заглушка)" });
    }
}