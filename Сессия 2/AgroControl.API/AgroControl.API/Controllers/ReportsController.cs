using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;

namespace AgroControl.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ReportsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string GetConnectionString() =>
            _configuration.GetConnectionString("DefaultConnection");

        private async Task<string> ExecuteStoredProcedure(string procedureName, params SqlParameter[] parameters)
        {
            var dt = new DataTable();
            using (var conn = new SqlConnection(GetConnectionString()))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(procedureName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(parameters);
                    using (var reader = await cmd.ExecuteReaderAsync())
                        dt.Load(reader);
                }
            }
            return JsonConvert.SerializeObject(dt);
        }

        [HttpGet("batches")]
        public async Task<IActionResult> BatchesReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var json = await ExecuteStoredProcedure("Отчет_ПартииЗаПериод",
                new SqlParameter("@ДатаНачала", startDate ?? new DateTime(2025, 3, 1)),
                new SqlParameter("@ДатаОкончания", endDate ?? new DateTime(2025, 3, 31)));
            return Content(json, "application/json");
        }

        [HttpGet("deviations")]
        public async Task<IActionResult> DeviationsReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var json = await ExecuteStoredProcedure("Отчет_ОтклоненияЗаПериод",
                new SqlParameter("@ДатаНачала", startDate ?? new DateTime(2025, 3, 1)),
                new SqlParameter("@ДатаОкончания", endDate ?? new DateTime(2025, 3, 31)));
            return Content(json, "application/json");
        }

        [HttpGet("recipe-usage")]
        public async Task<IActionResult> RecipeUsage([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var json = await ExecuteStoredProcedure("Отчет_ИспользованиеРецептур",
                new SqlParameter("@ДатаНачала", startDate ?? new DateTime(2025, 3, 1)),
                new SqlParameter("@ДатаОкончания", endDate ?? new DateTime(2025, 3, 31)));
            return Content(json, "application/json");
        }
    }
}