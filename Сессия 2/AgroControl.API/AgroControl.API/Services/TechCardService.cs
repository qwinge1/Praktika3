using Microsoft.EntityFrameworkCore;
using AgroControl.API.Models;

namespace AgroControl.API.Services
{
    public class TechCardService
    {
        private readonly AppDbContext _context;
        public TechCardService(AppDbContext context) => _context = context;

        public Task<List<TechCard>> GetAllAsync() =>
            _context.TechCards.Include(t => t.Шаги).ToListAsync();

        public Task<TechCard?> GetByIdAsync(int id) =>
            _context.TechCards.Include(t => t.Шаги).FirstOrDefaultAsync(t => t.ID == id);

        public async Task<TechCard> CreateAsync(TechCard card)
        {
            _context.TechCards.Add(card);
            await _context.SaveChangesAsync();
            return card;
        }

        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            var c = await _context.TechCards.FindAsync(id);
            if (c == null) return false;
            c.Статус = status;
            await _context.SaveChangesAsync();
            return true;
        }

        // Новый метод: полное обновление техкарты (без шагов)
        public async Task<bool> UpdateAsync(TechCard updated)
        {
            _context.Entry(updated).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        // Добавить шаг
        public async Task<TechCardStep> AddStepAsync(TechCardStep step)
        {
            _context.TechCardSteps.Add(step);
            await _context.SaveChangesAsync();
            return step;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var card = await _context.TechCards.FindAsync(id);
            if (card == null) return false;
            _context.TechCards.Remove(card);
            await _context.SaveChangesAsync();
            return true;
        }
        // Удалить шаг
        public async Task<bool> DeleteStepAsync(int techCardId, int stepId)
        {
            var step = await _context.TechCardSteps
                .FirstOrDefaultAsync(s => s.ID == stepId && s.ТехКартаID == techCardId);
            if (step == null) return false;
            _context.TechCardSteps.Remove(step);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}