using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgroControl.API.Models;

namespace AgroControl.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RawMaterialBatchesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RawMaterialBatchesController(AppDbContext context) => _context = context;

        // GET – список всех партий сырья
        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(new { success = true, data = await _context.RawMaterialBatches.ToListAsync() });

        // GET – партии, ожидающие лабораторного контроля
        [HttpGet("pending")]
        public async Task<IActionResult> GetPending()
        {
            var pending = await _context.RawMaterialBatches
                .Where(r => r.ЛабораторныйСтатус == "ожидает" || r.ЛабораторныйСтатус == "в работе")
                .ToListAsync();
            return Ok(new { success = true, data = pending });
        }

        // GET – одна партия по ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var batch = await _context.RawMaterialBatches.FindAsync(id);
            return batch == null
                ? NotFound(new { success = false, message = "Партия сырья не найдена" })
                : Ok(new { success = true, data = batch });
        }

        // POST – создать новую партию сырья
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RawMaterialBatch batch)
        {
            _context.RawMaterialBatches.Add(batch);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, data = batch });
        }

        // PUT – обновить партию сырья
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RawMaterialBatch updated)
        {
            var existing = await _context.RawMaterialBatches.FindAsync(id);
            if (existing == null) return NotFound(new { success = false, message = "Партия сырья не найдена" });

            existing.Поставщик = updated.Поставщик ?? existing.Поставщик;
            existing.Количество_кг = updated.Количество_кг ?? existing.Количество_кг;
            existing.ЛабораторныйСтатус = updated.ЛабораторныйСтатус ?? existing.ЛабораторныйСтатус;
            await _context.SaveChangesAsync();
            return Ok(new { success = true, data = existing });
        }

        // DELETE – удалить партию сырья
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var batch = await _context.RawMaterialBatches.FindAsync(id);
            if (batch == null) return NotFound(new { success = false, message = "Партия сырья не найдена" });
            _context.RawMaterialBatches.Remove(batch);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Партия сырья удалена" });
        }
    }
}