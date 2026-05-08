using Microsoft.AspNetCore.Mvc;

using AgroControl.API.Services;

using AgroControl.API.Models;

using Microsoft.EntityFrameworkCore;



namespace AgroControl.API.Controllers

{

    [ApiController]

    [Route("api/[controller]")]

    public class QualityControlController : ControllerBase

    {

        private readonly QualityControlService _service;

        private readonly AppDbContext _context;



        public QualityControlController(QualityControlService service, AppDbContext context)

        {

            _service = service;

            _context = context;

        }



        [HttpGet]

        public async Task<IActionResult> GetAll()

        {

            var tests = await _context.LabTests.ToListAsync();

            return Ok(new { success = true, data = tests });

        }



        [HttpGet("{id}")]

        public async Task<IActionResult> GetById(int id)

        {

            var test = await _context.LabTests.FindAsync(id);

            return test == null ? NotFound(new { success = false, message = "Испытание не найдено" }) : Ok(new { success = true, data = test });

        }



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



        [HttpPut("{id}")]

        public async Task<IActionResult> Update(int id, [FromBody] LabTest test)

        {

            var existing = await _context.LabTests.FindAsync(id);

            if (existing == null) return NotFound(new { success = false, message = "Испытание не найдено" });



            existing.ТипОбразца = test.ТипОбразца ?? existing.ТипОбразца;

            existing.НаименованиеПараметра = test.НаименованиеПараметра ?? existing.НаименованиеПараметра;

            existing.ИзмеренноеЗначение = test.ИзмеренноеЗначение ?? existing.ИзмеренноеЗначение;

            existing.НормативноеЗначение = test.НормативноеЗначение ?? existing.НормативноеЗначение;

            existing.ЕдиницаИзмерения = test.ЕдиницаИзмерения ?? existing.ЕдиницаИзмерения;
            existing.Результат = test.Результат ?? existing.Результат;

            existing.Решение = test.Решение ?? existing.Решение;

            existing.КомментарийАналитика = test.КомментарийАналитика ?? existing.КомментарийАналитика;

            await _context.SaveChangesAsync();

            return Ok(new { success = true, data = existing });

        }



        [HttpDelete("{id}")]

        public async Task<IActionResult> Delete(int id)

        {

            var test = await _context.LabTests.FindAsync(id);

            if (test == null) return NotFound(new { success = false });

            _context.LabTests.Remove(test);

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Испытание удалено" });

        }



        [HttpPut("{batchId}/decision")]

        public async Task<IActionResult> Decision(int batchId, [FromBody] DecisionDto dto)

        {

            var ok = await _service.MakeDecisionAsync(batchId, dto.Decision, dto.Comment);

            if (!ok) return NotFound(new { success = false, message = "Партия не найдена" });

            return Ok(new { success = true, message = "Решение принято" });

        }

    }



    public class DecisionDto

    {

        public string Decision { get; set; } = string.Empty;

        public string? Comment { get; set; }

    }

}