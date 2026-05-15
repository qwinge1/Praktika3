using AgroControl.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace AgroControl.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExtruderController : ControllerBase
    {
        private static readonly Random _random = new Random();

        [HttpGet("live")]
        public IActionResult GetLiveData()
        {
            var data = new ExtruderLiveData
            {
                ТемператураЗоны1 = _random.Next(75, 85),
                ТемператураЗоны2 = _random.Next(80, 90),
                Давление = Math.Round(_random.NextDouble() * 2 + 2.5, 2),
                СкоростьШнека = _random.Next(300, 500),
                ТекущаяМощность = _random.Next(40, 60),
                ВремяРаботы = DateTime.Now.ToString("HH:mm:ss")
            };
            return Ok(new { success = true, data });
        }
    }
}