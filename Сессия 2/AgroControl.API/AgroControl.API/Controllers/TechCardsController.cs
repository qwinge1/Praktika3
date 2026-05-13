using Microsoft.AspNetCore.Mvc;

using AgroControl.API.Models;

using AgroControl.API.Services;



namespace AgroControl.API.Controllers

{

    [ApiController]

    [Route("api/[controller]")]

    public class TechCardsController : ControllerBase

    {

        private readonly TechCardService _service;

        public TechCardsController(TechCardService service) => _service = service;



        [HttpGet]

        public async Task<IActionResult> GetAll() =>

            Ok(new { success = true, data = await _service.GetAllAsync() });



        [HttpGet("{id}")]

        public async Task<IActionResult> GetById(int id)

        {

            var card = await _service.GetByIdAsync(id);

            return card == null

                ? NotFound(new { success = false, message = "Технологическая карта не найдена" })

                : Ok(new { success = true, data = card });

        }



        [HttpPost]

        public async Task<IActionResult> Create([FromBody] TechCard card)

        {

            var created = await _service.CreateAsync(card);

            return Ok(new { success = true, data = created });

        }



        [HttpPut("{id}")]

        public async Task<IActionResult> Update(int id, [FromBody] TechCard updated)

        {

            var existing = await _service.GetByIdAsync(id);

            if (existing == null)

                return NotFound(new { success = false, message = "Технологическая карта не найдена" });



            existing.Версия = updated.Версия;

            existing.Статус = updated.Статус;

            existing.ПродуктID = updated.ПродуктID;

            await _service.UpdateAsync(existing);

            return Ok(new { success = true, data = existing });

        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound(new { success = false, message = "Техкарта не найдена" });
            return Ok(new { success = true, message = "Техкарта удалена" });
        }

        [HttpPut("{id}/status")]

        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)

        {

            var ok = await _service.UpdateStatusAsync(id, status);

            return ok

                ? Ok(new { success = true, message = "Статус обновлён" })

                : NotFound(new { success = false, message = "Технологическая карта не найдена" });

        }



        [HttpPost("{id}/steps")]

        public async Task<IActionResult> AddStep(int id, [FromBody] TechCardStep step)

        {

            var card = await _service.GetByIdAsync(id);

            if (card == null)

                return NotFound(new { success = false, message = "Технологическая карта не найдена" });



            step.ТехКартаID = id;

            await _service.AddStepAsync(step);

            return Ok(new { success = true, data = step });

        }



        [HttpDelete("{id}/steps/{stepId}")]

        public async Task<IActionResult> DeleteStep(int id, int stepId)

        {

            var ok = await _service.DeleteStepAsync(id, stepId);

            return ok

                ? Ok(new { success = true, message = "Шаг удалён" })

                : NotFound(new { success = false, message = "Шаг не найден" });

        }



        [HttpGet("{id}/program")]

        public async Task<IActionResult> GetProgram(int id)

        {

            var card = await _service.GetByIdAsync(id);

            if (card == null)

                return NotFound(new { success = false, message = "Технологическая карта не найдена" });



            var steps = card.Шаги?.OrderBy(s => s.НомерШага).ToList();

            return Ok(new { success = true, data = new { card.ID, card.Версия, Steps = steps } });

        }

    }

}