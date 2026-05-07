using Microsoft.EntityFrameworkCore;
using AgroControl.API.Models;

namespace AgroControl.API.Services
{
    public class RecipeService
    {
        private readonly AppDbContext _context;
        public RecipeService(AppDbContext context) => _context = context;

        public Task<List<Recipe>> GetAllAsync() =>
            _context.Recipes.Include(r => r.Состав).ToListAsync();

        public Task<Recipe?> GetByIdAsync(int id) =>
            _context.Recipes.Include(r => r.Состав).FirstOrDefaultAsync(r => r.ID == id);

        public async Task<Recipe> CreateAsync(Recipe recipe)
        {
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();
            return recipe;
        }

        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            var r = await _context.Recipes.FindAsync(id);
            if (r == null) return false;
            r.Статус = status;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}