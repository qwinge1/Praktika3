using AgroControl.API.Models;
using AgroControl.API.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class RecipesController : ControllerBase
{
    private readonly RecipeService _service;
    public RecipesController(RecipeService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(new { success = true, data = await _service.GetAllAsync() });

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id) => Ok(new { success = true, data = await _service.GetByIdAsync(id) });

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Recipe recipe)
    {
        var created = await _service.CreateAsync(recipe);
        return Ok(new { success = true, data = created });
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
    {
        var ok = await _service.UpdateStatusAsync(id, status);
        return ok ? Ok(new { success = true }) : NotFound(new { success = false });
    }
}