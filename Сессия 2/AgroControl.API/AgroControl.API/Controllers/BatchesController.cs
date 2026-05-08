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



        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateBatch(int id, [FromBody] BatchUpdateDto dto)

        {

            var ok = await _batchService.UpdateBatchAsync(id, dto.StartTime, dto.EndTime, dto.Status, dto.FactQuantity);

            if (!ok) return NotFound(new { success = false, message = "Партия не найдена" });

            return Ok(new { success = true, message = "Партия обновлена" });

        }



        [HttpDelete("{id}")]

        public async Task<IActionResult> Delete(int id) =>

            Ok(new { success = true, message = "Удаление партии (заглушка)" });

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