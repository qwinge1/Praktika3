using AgroControl.API.Models;
using AgroControl.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class RecipesController : ControllerBase
{
    private readonly RecipeService _service;

    public RecipesController(RecipeService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(new { success = true, data = await _service.GetAllAsync() });

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var recipe = await _service.GetByIdAsync(id);
        return recipe == null ? NotFound(new { success = false, message = "Рецептура не найдена" }) : Ok(new { success = true, data = recipe });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Recipe recipe)
    {
        var created = await _service.CreateAsync(recipe);
        return Ok(new { success = true, data = created });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) return NotFound(new { success = false, message = "Рецепт не найден" });
        return Ok(new { success = true, message = "Рецепт удалён" });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Recipe updated)
    {
        updated.ID = id;
        await _service.UpdateAsync(updated);
        var recipe = await _service.GetByIdAsync(id);
        return Ok(new { success = true, data = recipe });
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
    {
        var ok = await _service.UpdateStatusAsync(id, status);
        return ok ? Ok(new { success = true }) : NotFound(new { success = false });
    }

    [HttpPost("{id}/components")]
    public async Task<IActionResult> AddComponent(int id, [FromBody] RecipeComponent comp)
    {
        var added = await _service.AddComponentAsync(id, comp);
        return Ok(new { success = true, data = added });
    }

    [HttpDelete("{id}/components/{componentId}")]
    public async Task<IActionResult> DeleteComponent(int id, int componentId)
    {
        var ok = await _service.DeleteComponentAsync(id, componentId);
        return ok ? Ok(new { success = true }) : NotFound(new { success = false });
    }
}