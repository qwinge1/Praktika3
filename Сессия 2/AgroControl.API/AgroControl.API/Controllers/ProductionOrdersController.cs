using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgroControl.API.Models;

[ApiController]
[Route("api/[controller]")]
public class ProductionOrdersController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductionOrdersController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(new { success = true, data = await _context.ProductionOrders.ToListAsync() });

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _context.ProductionOrders.FindAsync(id);
        return order == null ? NotFound() : Ok(new { success = true, data = order });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductionOrder order)
    {
        _context.ProductionOrders.Add(order);
        await _context.SaveChangesAsync();
        return Ok(new { success = true, data = order });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProductionOrder updated)
    {
        var existing = await _context.ProductionOrders.FindAsync(id);
        if (existing == null) return NotFound();

        existing.НомерЗаказа = updated.НомерЗаказа;
        existing.ПродуктID = updated.ПродуктID;
        existing.РецептID = updated.РецептID;
        existing.ТехКартаID = updated.ТехКартаID;
        existing.ПланКоличество_кг = updated.ПланКоличество_кг;
        existing.Статус = updated.Статус;
        existing.ПланДатаСтарта = updated.ПланДатаСтарта;
        await _context.SaveChangesAsync();
        return Ok(new { success = true, data = existing });
    }
}