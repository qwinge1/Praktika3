using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AgroControl.API.Services;

namespace AgroControl.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class BatchesController : ControllerBase
    {
        private readonly BatchService _batchService;
        public BatchesController(BatchService batchService) => _batchService = batchService;

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(new { success = true, data = await _batchService.GetAllAsync() });

        [HttpGet("active")]
        public async Task<IActionResult> GetActive() =>
            Ok(new { success = true, data = await _batchService.GetActiveAsync() });

        [HttpPost("{id}/start")]
        public async Task<IActionResult> Start(int id)
        {
            var batch = await _batchService.StartAsync(id);
            if (batch == null) return NotFound();
            return Ok(new { success = true, message = "Партия запущена" });
        }
        [HttpPut("steps/{stepId}/actuals")]
        public async Task<IActionResult> UpdateActuals(int stepId, [FromBody] BatchStepActualsDto dto)
        {
            var step = await _batchService.UpdateActualsAsync(stepId, dto.ActualTemp, dto.ActualDuration, dto.ActualPressure, dto.Comment);
            if (step == null) return NotFound(new { success = false, message = "Шаг не найден" });
            return Ok(new { success = true, data = step });
        }

        // Вспомогательный DTO (можно поместить в папку Models)
        public class BatchStepActualsDto
        {
            public decimal? ActualTemp { get; set; }
            public int? ActualDuration { get; set; }
            public decimal? ActualPressure { get; set; }
            public string? Comment { get; set; }
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

    }
}