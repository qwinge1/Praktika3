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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var batches = await _context.RawMaterialBatches.Include(b => b.Сырье).ToListAsync();
            return Ok(new { success = true, data = batches });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var batch = await _context.RawMaterialBatches.Include(b => b.Сырье).FirstOrDefaultAsync(b => b.ID == id);
            if (batch == null) return NotFound();
            return Ok(new { success = true, data = batch });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RawMaterialBatch updated)
        {
            var existing = await _context.RawMaterialBatches.FindAsync(id);
            if (existing == null) return NotFound();
            existing.ЛабораторныйСтатус = updated.ЛабораторныйСтатус;
            existing.КомментарийРешения = updated.КомментарийРешения;
            existing.РешениеПринял = updated.РешениеПринял;
            existing.ДатаРешения = DateTime.Now;
            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }
    }
}