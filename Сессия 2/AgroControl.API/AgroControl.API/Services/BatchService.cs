using Microsoft.EntityFrameworkCore;
using AgroControl.API.Models;

namespace AgroControl.API.Services
{
    public class BatchService
    {
        private readonly AppDbContext _context;
        public BatchService(AppDbContext context) => _context = context;

        public Task<List<ProductionBatch>> GetAllAsync() =>
            _context.ProductionBatches.Include(b => b.ВыполнениеШагов).ToListAsync();

        public Task<List<ProductionBatch>> GetActiveAsync() =>
            _context.ProductionBatches
                .Where(b => b.Статус == "выполняется" || b.Статус == "запланирована")
                .Include(b => b.ВыполнениеШагов)
                .ToListAsync();

        public async Task<ProductionBatch?> GetByIdAsync(int id) =>
            await _context.ProductionBatches.Include(b => b.ВыполнениеШагов).FirstOrDefaultAsync(b => b.ID == id);

        public async Task<ProductionBatch?> StartAsync(int id)
        {
            var batch = await _context.ProductionBatches.FindAsync(id);
            if (batch == null) return null;
            batch.Статус = "выполняется";
            batch.ВремяСтарта = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return batch;
        }

        public async Task<BatchStepExecution?> StartStepAsync(int stepId)
        {
            var step = await _context.BatchStepExecutions.FindAsync(stepId);
            if (step == null) return null;
            step.ВремяСтарта = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return step;
        }

        public async Task<BatchStepExecution?> CompleteStepAsync(int stepId)
        {
            var step = await _context.BatchStepExecutions.FindAsync(stepId);
            if (step == null) return null;
            step.ВремяОкончания = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return step;
        }

        public async Task<BatchStepExecution?> UpdateActualsAsync(int stepId, decimal? actualTemp, int? actualDuration, decimal? actualPressure, string? comment)
        {
            var step = await _context.BatchStepExecutions
                .Include(s => s.ШагТехКарты)            // подгружаем плановые параметры
                .FirstOrDefaultAsync(s => s.ID == stepId);
            if (step == null) return null;

            step.ФактТемпература = actualTemp;
            step.ФактДлительностьМинут = actualDuration;
            step.ФактДавление = actualPressure;
            step.КомментарийОператора = comment;

            // Расчёт отклонения с учётом плановых значений
            var plan = step.ШагТехКарты;
            step.Отклонение = (actualTemp != null && plan?.ПланТемпература != null && Math.Abs(actualTemp.Value - plan.ПланТемпература.Value) > 1.0m)
                             || (actualDuration != null && plan?.ПланДлительностьМинут != null && Math.Abs(actualDuration.Value - plan.ПланДлительностьМинут.Value) > 5)
                             || (actualPressure != null && plan?.ПланДавление != null && Math.Abs(actualPressure.Value - plan.ПланДавление.Value) > 0.2m);

            await _context.SaveChangesAsync();
            return step;
        }
        public async Task<ProductionBatch> CreateAsync(ProductionBatch batch)
        {
            if (string.IsNullOrEmpty(batch.Статус))
                batch.Статус = "запланирована";

            batch.ВремяСтарта = null;
            batch.ВремяОкончания = null;
            batch.ФактКоличество_кг = null;
            batch.ТекущийШагID = null;

            // Проверка: существует ли заказ с таким ID
            var orderExists = await _context.ProductionOrders.AnyAsync(o => o.ID == batch.ЗаказID);
            if (!orderExists)
                throw new ArgumentException($"Заказ с ID {batch.ЗаказID} не найден в БД");

            _context.ProductionBatches.Add(batch);
            await _context.SaveChangesAsync();
            return batch;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var batch = await _context.ProductionBatches.FindAsync(id);
            if (batch == null) return false;
            _context.ProductionBatches.Remove(batch);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateBatchAsync(int id, DateTime? start, DateTime? end, string? status, decimal? factQty)
        {
            var batch = await _context.ProductionBatches.FindAsync(id);
            if (batch == null) return false;
            if (start.HasValue) batch.ВремяСтарта = start;
            if (end.HasValue) batch.ВремяОкончания = end;
            if (!string.IsNullOrEmpty(status)) batch.Статус = status;
            if (factQty.HasValue) batch.ФактКоличество_кг = factQty;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}