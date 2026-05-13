using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgroControl.API.Models;

namespace AgroControl.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductionBatchesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductionBatchesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var batches = await _context.ProductionBatches.ToListAsync();
            return Ok(new { success = true, data = batches });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var batch = await _context.ProductionBatches.FindAsync(id);
            if (batch == null) return NotFound();
            return Ok(new { success = true, data = batch });
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var batch = await _context.ProductionBatches.FindAsync(id);
            if (batch == null) return NotFound();
            batch.Статус = status;
            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }
    }
}