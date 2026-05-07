using Microsoft.AspNetCore.Mvc;
using AgroControl.API.Services;
using AgroControl.API.Models;

namespace AgroControl.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QualityControlController : ControllerBase
    {
        private readonly QualityControlService _service;
        public QualityControlController(QualityControlService service) => _service = service;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LabTest test)
        {
            try
            {
                var created = await _service.CreateTestAsync(test);
                return Ok(new { success = true, data = created });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        [HttpPut("{batchId}/decision")]
        public async Task<IActionResult> Decision(int batchId, [FromBody] DecisionDto dto)
        {
            if (string.IsNullOrEmpty(dto.Decision))
                return BadRequest(new { success = false, message = "Решение не указано" });

            var ok = await _service.MakeDecisionAsync(batchId, dto.Decision, dto.Comment);
            if (!ok) return NotFound(new { success = false, message = "Партия не найдена" });
            return Ok(new { success = true, message = "Решение принято" });
        }
    }

    public class DecisionDto
    {
        public string Decision { get; set; } = string.Empty;   // "одобрено" или "заблокировано"
        public string? Comment { get; set; }
    }
}