using Microsoft.AspNetCore.Mvc;
using AgroControl.API.Services;
using AgroControl.API.Models;

namespace AgroControl.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ReferenceService _refs;
        public ProductsController(ReferenceService refs) => _refs = refs;

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(new { success = true, data = await _refs.GetProductsAsync() });

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _refs.GetProductAsync(id);
            return product == null
                ? NotFound(new { success = false, message = "Продукт не найден" })
                : Ok(new { success = true, data = product });
        }

        [HttpPost]
        public IActionResult Create([FromBody] Product product) =>
            Ok(new { success = true, message = "Продукт создан (заглушка)" });

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Product product) =>
            Ok(new { success = true, message = "Продукт обновлён (заглушка)" });

        [HttpDelete("{id}")]
        public IActionResult Delete(int id) =>
            Ok(new { success = true, message = "Продукт удалён (заглушка)" });

        [HttpPut("{id}/archive")]
        public async Task<IActionResult> Archive(int id)
        {
            var ok = await _refs.ArchiveProductAsync(id);
            return ok ? Ok(new { success = true }) : NotFound(new { success = false, message = "Продукт не найден" });
        }
    }
}