using Microsoft.AspNetCore.Mvc;



namespace AgroControl.API.Controllers

{

    [ApiController]

    [Route("api/[controller]")]

    public class ReportsController : ControllerBase

    {

        [HttpGet("batches")]

        public IActionResult BatchesReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate) =>

            Ok(new { success = true, message = "Отчёт по партиям (хранимая процедура Отчет_ПартииЗаПериод)", startDate, endDate });



        [HttpGet("deviations")]

        public IActionResult DeviationsReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate) =>

            Ok(new { success = true, message = "Отчёт по отклонениям (Отчет_ОтклоненияЗаПериод)", startDate, endDate });



        [HttpGet("recipe-usage")]

        public IActionResult RecipeUsage([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate) =>

            Ok(new { success = true, message = "Отчёт по использованию рецептур (Отчет_ИспользованиеРецептур)", startDate, endDate });

    }

}