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
        public async Task<BatchStepExecution?> UpdateActualsAsync(int stepId, decimal? actualTemp, int? actualDuration, decimal? actualPressure, string? comment)
        {
            var step = await _context.BatchStepExecutions.FindAsync(stepId);
            if (step == null) return null;

            // Загружаем соответствующий шаг техкарты, чтобы получить плановые значения
            var techStep = await _context.TechCardSteps.FindAsync(step.ШагТехКартыID);

            step.ФактТемпература = actualTemp;
            step.ФактДлительностьМинут = actualDuration;
            step.ФактДавление = actualPressure;
            step.КомментарийОператора = comment;

            // Рассчитываем отклонения, если есть плановые значения
            step.Отклонение = (actualTemp != null && techStep?.ПланТемпература != null && Math.Abs(actualTemp.Value - techStep.ПланТемпература.Value) > 1.0m)
                             || (actualDuration != null && techStep?.ПланДлительностьМинут != null && Math.Abs(actualDuration.Value - techStep.ПланДлительностьМинут.Value) > 5)
                             || (actualPressure != null && techStep?.ПланДавление != null && Math.Abs(actualPressure.Value - techStep.ПланДавление.Value) > 0.2m);

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
    }
}