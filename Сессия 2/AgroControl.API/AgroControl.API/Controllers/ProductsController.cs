using Microsoft.AspNetCore.Mvc;
using AgroControl.API.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AgroControl.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll() =>
            Ok(new { success = true, data = _context.Products.ToList() });

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound(new { success = false, message = "Продукт не найден" });
            return Ok(new { success = true, data = product });
        }

        [HttpPost]
        public IActionResult Create([FromBody] Product product)
        {
            if (_context.Products.Any(p => p.Код == product.Код))
                return BadRequest(new { success = false, message = "Продукт с таким кодом уже существует" });
            _context.Products.Add(product);
            _context.SaveChanges();
            return Ok(new { success = true, data = product });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Product updated)
        {
            var existing = _context.Products.Find(id);
            if (existing == null) return NotFound(new { success = false, message = "Продукт не найден" });
            existing.Код = updated.Код;
            existing.Наименование = updated.Наименование;
            existing.Тип = updated.Тип;
            existing.ФормаВыпуска = updated.ФормаВыпуска;
            existing.Статус = updated.Статус;
            _context.SaveChanges();
            return Ok(new { success = true, data = existing });
        }

        [HttpPut("{id}/archive")]
        public IActionResult Archive(int id)
        {
            var existing = _context.Products.Find(id);
            if (existing == null) return NotFound(new { success = false, message = "Продукт не найден" });
            existing.Статус = "архив";
            _context.SaveChanges();
            return Ok(new { success = true, data = existing });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound(new { success = false, message = "Продукт не найден" });
            _context.Products.Remove(product);
            _context.SaveChanges();
            return Ok(new { success = true, message = "Продукт удалён" });
        }
    }
}