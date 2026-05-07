using Microsoft.AspNetCore.Mvc;
using AgroControl.API.Services;

namespace AgroControl.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ReferenceService _refs;
        public ProductsController(ReferenceService refs) => _refs = refs;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(new { success = true, data = await _refs.GetProductsAsync() });

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id) => Ok(new { success = true, data = await _refs.GetProductAsync(id) });

        [HttpPut("{id}/archive")]
        public async Task<IActionResult> Archive(int id)
        {
            var ok = await _refs.ArchiveProductAsync(id);
            return ok ? Ok(new { success = true }) : NotFound(new { success = false, message = "Продукт не найден" });
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class MaterialsController : ControllerBase
    {
        private readonly ReferenceService _refs;
        public MaterialsController(ReferenceService refs) => _refs = refs;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(new { success = true, data = await _refs.GetMaterialsAsync() });

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id) => Ok(new { success = true, data = await _refs.GetMaterialAsync(id) });
    }

    [ApiController]
    [Route("api/[controller]")]
    public class EquipmentController : ControllerBase
    {
        private readonly ReferenceService _refs;
        public EquipmentController(ReferenceService refs) => _refs = refs;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(new { success = true, data = await _refs.GetEquipmentAsync() });

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id) => Ok(new { success = true, data = await _refs.GetEquipmentAsync(id) });
    }

    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ReferenceService _refs;
        public UsersController(ReferenceService refs) => _refs = refs;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(new { success = true, data = await _refs.GetUsersAsync() });

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id) => Ok(new { success = true, data = await _refs.GetUserAsync(id) });
    }
}