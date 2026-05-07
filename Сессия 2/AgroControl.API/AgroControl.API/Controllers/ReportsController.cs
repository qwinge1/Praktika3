using Microsoft.AspNetCore.Mvc;
using AgroControl.API.Models;

namespace AgroControl.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context) => _context = context;

        [HttpGet("batches")]
        public IActionResult BatchesReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            // В рабочем проекте здесь можно вызвать хранимую процедуру
            return Ok(new { success = true, message = "Отчёт по партиям будет доступен после вызова хранимой процедуры", startDate, endDate });
        }

        [HttpGet("deviations")]
        public IActionResult DeviationsReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            return Ok(new { success = true, message = "Отчёт по отклонениям будет реализован позже", startDate, endDate });
        }

        [HttpGet("recipe-usage")]
        public IActionResult RecipeUsageReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            return Ok(new { success = true, message = "Отчёт по использованию рецептур доступен через хранимую процедуру", startDate, endDate });
        }
    }
}