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
            IQueryable<LabTest> query = _context.LabTests;
            if (batchId.HasValue)
            {
                // Ищем испытания, связанные с партией сырья или готовой продукции
                query = query.Where(t => t.ПартияСырьяID == batchId || t.ПартияПроизводстваID == batchId);
            }
            var tests = await query.ToListAsync();
            return Ok(new { success = true, data = tests });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LabTest test)
        {
            _context.LabTests.Add(test);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, data = test });
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
            existing.КомментарийАналитика = updated.КомментарийАналитика;
            existing.Результат = updated.Результат;
            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }
    }
}