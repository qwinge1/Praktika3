using Microsoft.EntityFrameworkCore;
using AgroControl.API.Models;

namespace AgroControl.API.Services
{
    public class ReferenceService
    {
        private readonly AppDbContext _context;

        public ReferenceService(AppDbContext context) => _context = context;

        public Task<List<Product>> GetProductsAsync() => _context.Products.ToListAsync();
        public Task<List<RawMaterial>> GetMaterialsAsync() => _context.RawMaterials.ToListAsync();
        public Task<List<Equipment>> GetEquipmentAsync() => _context.Equipment.ToListAsync();
        public Task<List<User>> GetUsersAsync() => _context.Users.ToListAsync();
        public Task<Product?> GetProductAsync(int id) => _context.Products.FindAsync(id).AsTask();
        public Task<RawMaterial?> GetMaterialAsync(int id) => _context.RawMaterials.FindAsync(id).AsTask();
        public Task<Equipment?> GetEquipmentAsync(int id) => _context.Equipment.FindAsync(id).AsTask();
        public Task<User?> GetUserAsync(int id) => _context.Users.FindAsync(id).AsTask();

        public async Task<bool> ArchiveProductAsync(int id)
        {
            var p = await _context.Products.FindAsync(id);
            if (p == null) return false;
            p.Статус = "архив";
            await _context.SaveChangesAsync();
            return true;
        }
    }
}