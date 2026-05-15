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

        [HttpGet("active")]
        public async Task<IActionResult> GetActive() =>
            Ok(new { success = true, data = await _batchService.GetActiveBatchesWithDetailsAsync() });

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

        [HttpGet("{batchId}/program")]
        public async Task<IActionResult> GetProgram(int batchId)
        {
            var program = await _batchService.GetProgramAsync(batchId);
            if (program == null) return NotFound(new { success = false, message = "Партия не найдена" });
            return Ok(new { success = true, data = program });
        }

        [HttpGet("{batchId}/events")]
        public async Task<IActionResult> GetEvents(int batchId)
        {
            var events = await _batchService.GetEventsAsync(batchId);
            return Ok(new { success = true, data = events });
        }

        [HttpPost("{batchId}/report-issue")]
        public async Task<IActionResult> ReportIssue(int batchId, [FromBody] ReportIssueDto dto)
        {
            var result = await _batchService.ReportIssueAsync(batchId, dto.Message, dto.OperatorId);
            if (!result) return NotFound(new { success = false, message = "Партия не найдена" });
            return Ok(new { success = true, message = "Проблема зарегистрирована" });
        }
        [HttpPut("{id}/complete")]
        public async Task<IActionResult> CompleteBatch(int id, [FromBody] CompleteBatchDto dto)
        {
            var batch = await _context.ProductionBatches.FindAsync(id);
            if (batch == null) return NotFound();
            batch.Статус = dto.Status;
            batch.ФактКоличество_кг = dto.FactQuantity;
            batch.ВремяОкончания = DateTime.Now;
            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductionBatch batch)
        {
            var created = await _batchService.CreateAsync(batch);
            return Ok(new { success = true, data = created });
        }
        // В Controllers/BatchesController.cs добавить:
        [HttpGet("issues")]
        public async Task<IActionResult> GetAllIssues()
        {
            var events = await _context.EventLogs
                .Where(e => e.ТипСобытия == "проблема")
                .Include(e => e.Создал)
                .OrderByDescending(e => e.ВремяСобытия)
                .ToListAsync();
            return Ok(new { success = true, data = events });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _batchService.DeleteAsync(id);
            if (!ok) return NotFound(new { success = false, message = "Партия не найдена" });
            return Ok(new { success = true, message = "Партия удалена" });
        }
    }
    public class CompleteBatchDto
    {
        public decimal FactQuantity { get; set; }
        public string Status { get; set; }
    }
    public class BatchStepActualsDto
    {
        public decimal? ActualTemp { get; set; }
        public int? ActualDuration { get; set; }
        public decimal? ActualPressure { get; set; }
        public string? Comment { get; set; }
    }

    public class ReportIssueDto
    {
        public string Message { get; set; } = string.Empty;
        public int OperatorId { get; set; }
    }
}