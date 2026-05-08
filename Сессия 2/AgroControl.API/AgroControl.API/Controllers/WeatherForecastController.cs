using Microsoft.AspNetCore.Mvc;

namespace AgroControl.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private static List<WeatherForecast> forecasts = new();

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        // GET Ц получить все прогнозы (или сгенерировать)
        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            // ƒл€ демонстрации возвращаем 5 случайных прогнозов
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        // POST Ц создать новый прогноз
        [HttpPost]
        public IActionResult Post([FromBody] WeatherForecast forecast)
        {
            forecasts.Add(forecast);
            return Ok(new { success = true, data = forecast });
        }

        // PUT Ц обновить прогноз (условно по дате)
        [HttpPut("{date}")]
        public IActionResult Put(string date, [FromBody] WeatherForecast updated)
        {
            // здесь можно искать по дате и обновл€ть
            return Ok(new { success = true, message = "ѕрогноз обновлЄн (заглушка)" });
        }

        // DELETE Ц удалить прогноз (условно по дате)
        [HttpDelete("{date}")]
        public IActionResult Delete(string date)
        {
            // здесь можно удал€ть по дате
            return Ok(new { success = true, message = "ѕрогноз удалЄн (заглушка)" });
        }
    }
}