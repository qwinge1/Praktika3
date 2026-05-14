using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgroControl.API.Models;
using AgroControl.API.Services;

namespace AgroControl.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BatchesController : ControllerBase
    {
        private readonly BatchService _batchService;
        private readonly AppDbContext _context;

        public BatchesController(BatchService batchService, AppDbContext context)
        {
            _batchService = batchService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(new { success = true, data = await _batchService.GetAllAsync() });

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductionBatch batch)
        {
            var created = await _batchService.CreateAsync(batch);
            return Ok(new { success = true, data = created });
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive() =>
            Ok(new { success = true, data = await _batchService.GetActiveAsync() });

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var batch = await _batchService.GetByIdAsync(id);
            return batch == null ? NotFound(new { success = false, message = "Партия не найдена" }) : Ok(new { success = true, data = batch });
        }

        [HttpPost("{id}/start")]
        public async Task<IActionResult> Start(int id)
        {
            var batch = await _batchService.StartAsync(id);
            if (batch == null) return NotFound();
            return Ok(new { success = true, message = "Партия запущена" });
        }

        [HttpPost("steps/{stepId}/start")]
        public async Task<IActionResult> StartStep(int stepId)
        {
            var step = await _batchService.StartStepAsync(stepId);
            if (step == null) return NotFound();
            return Ok(new { success = true, message = "Шаг начат" });
        }

        [HttpPost("steps/{stepId}/complete")]
        public async Task<IActionResult> CompleteStep(int stepId)
        {
            var step = await _batchService.CompleteStepAsync(stepId);
            if (step == null) return NotFound();
            return Ok(new { success = true, message = "Шаг завершён" });
        }

        [HttpPut("steps/{stepId}/actuals")]
        public async Task<IActionResult> UpdateActuals(int stepId, [FromBody] BatchStepActualsDto dto)
        {
            var step = await _batchService.UpdateActualsAsync(stepId, dto.ActualTemp, dto.ActualDuration, dto.ActualPressure, dto.Comment);
            if (step == null) return NotFound(new { success = false, message = "Шаг не найден" });
            return Ok(new { success = true, data = step });
        }

        // ИСПРАВЛЕННЫЙ метод для редактирования партии (принимает ProductionBatch)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBatch(int id, [FromBody] ProductionBatch updated)
        {
            var existing = await _context.ProductionBatches.FindAsync(id);
            if (existing == null) return NotFound(new { success = false, message = "Партия не найдена" });
            existing.НомерПартии = updated.НомерПартии;
            existing.Статус = updated.Статус;
            existing.ФактКоличество_кг = updated.ФактКоличество_кг;
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Партия обновлена" });
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

        [HttpPut("{id}/lab-status")]
        public async Task<IActionResult> UpdateLabStatus(int id, [FromBody] ProductionBatch updated)
        {
            var existing = await _context.ProductionBatches.FindAsync(id);
            if (existing == null) return NotFound();
            existing.ЛабораторныйСтатус = updated.ЛабораторныйСтатус;
            existing.КомментарийРешения = updated.КомментарийРешения;
            existing.РешениеПринял = updated.РешениеПринял;
            existing.ДатаРешения = DateTime.Now;
            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _batchService.DeleteAsync(id);
            if (!ok) return NotFound(new { success = false, message = "Партия не найдена" });
            return Ok(new { success = true, message = "Партия удалена" });
        }
    }

    public class BatchUpdateDto
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Status { get; set; }
        public decimal? FactQuantity { get; set; }
    }

    public class BatchStepActualsDto
    {
        public decimal? ActualTemp { get; set; }
        public int? ActualDuration { get; set; }
        public decimal? ActualPressure { get; set; }
        public string? Comment { get; set; }
    }
}