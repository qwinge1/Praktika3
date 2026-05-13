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

        // GET: api/RawMaterialBatches
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var batches = await _context.RawMaterialBatches.ToListAsync();
                return Ok(new { success = true, data = batches });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // GET: api/RawMaterialBatches/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var batch = await _context.RawMaterialBatches.FindAsync(id);
            if (batch == null) return NotFound();
            return Ok(new { success = true, data = batch });
        }

        // PUT: api/RawMaterialBatches/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RawMaterialBatch updated)
        {
            var existing = await _context.RawMaterialBatches.FindAsync(id);
            if (existing == null) return NotFound();
            existing.ЛабораторныйСтатус = updated.ЛабораторныйСтатус;
            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }
    }
}