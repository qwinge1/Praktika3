using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgroControl.API.Models;

namespace AgroControl.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QualityControlController : ControllerBase
    {
        private readonly AppDbContext _context;
        public QualityControlController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? batchId)
        {
            try
            {
                IQueryable<LabTest> query = _context.LabTests.Include(t => t.Исполнитель);
                if (batchId.HasValue)
                    query = query.Where(t => t.ПартияСырьяID == batchId || t.ПартияПроизводстваID == batchId);
                var tests = await query.ToListAsync();
                return Ok(new { success = true, data = tests });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LabTest test)
        {
            try
            {
                if (test.ПартияСырьяID == null && test.ПартияПроизводстваID == null)
                    return BadRequest(new { success = false, message = "Не указана партия" });
                _context.LabTests.Add(test);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, data = test });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] LabTest updated)
        {
            var existing = await _context.LabTests.FindAsync(id);
            if (existing == null) return NotFound();
            existing.НаименованиеПараметра = updated.НаименованиеПараметра;
            existing.НормативноеЗначение = updated.НормативноеЗначение;
            existing.ИзмеренноеЗначение = updated.ИзмеренноеЗначение;
            existing.ЕдиницаИзмерения = updated.ЕдиницаИзмерения;
            existing.КомментарийЛаборанта = updated.КомментарийЛаборанта;
            existing.Результат = updated.Результат;
            existing.Приоритет = updated.Приоритет;
            existing.ИсполнительID = updated.ИсполнительID;
            existing.ДатаНазначения = updated.ДатаНазначения;
            existing.ТипОбразца = updated.ТипОбразца;
            existing.Статус = updated.Статус;
            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var test = await _context.LabTests.FindAsync(id);
            if (test == null) return NotFound();
            _context.LabTests.Remove(test);
            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }
    }
}