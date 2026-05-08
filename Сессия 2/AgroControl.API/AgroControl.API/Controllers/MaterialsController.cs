using Microsoft.AspNetCore.Mvc;
using AgroControl.API.Services;
using AgroControl.API.Models;

namespace AgroControl.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaterialsController : ControllerBase
    {
        private readonly ReferenceService _refs;
        public MaterialsController(ReferenceService refs) => _refs = refs;

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(new { success = true, data = await _refs.GetMaterialsAsync() });

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var mat = await _refs.GetMaterialAsync(id);
            return mat == null
                ? NotFound(new { success = false, message = "Материал не найден" })
                : Ok(new { success = true, data = mat });
        }

        [HttpPost]
        public IActionResult Create([FromBody] RawMaterial material) =>
            Ok(new { success = true, message = "Материал создан (заглушка)" });

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] RawMaterial material) =>
            Ok(new { success = true, message = "Материал обновлён (заглушка)" });

        [HttpDelete("{id}")]
        public IActionResult Delete(int id) =>
            Ok(new { success = true, message = "Материал удалён (заглушка)" });
    }
}