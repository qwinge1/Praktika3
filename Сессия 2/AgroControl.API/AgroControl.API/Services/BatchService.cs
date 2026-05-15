using AgroControl.API.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AgroControl.API.Services
{
    public class BatchService
    {
        private readonly AppDbContext _context;
        public BatchService(AppDbContext context) => _context = context;

        public Task<List<ProductionBatch>> GetAllAsync() =>
            _context.ProductionBatches.Include(b => b.ВыполнениеШагов).ToListAsync();

        // Новый метод для активных партий с деталями
        public async Task<List<ActiveBatchDto>> GetActiveBatchesWithDetailsAsync()
        {
            var batches = await _context.ProductionBatches
                .Where(b => b.Статус == "выполняется" || b.Статус == "запланирована")
                .Include(b => b.Заказ)
                    .ThenInclude(o => o!.ТехКарта)
                        .ThenInclude(tc => tc!.Шаги)
                .Include(b => b.ВыполнениеШагов)
                .ToListAsync();

            var result = new List<ActiveBatchDto>();
            foreach (var batch in batches)
            {
                var order = batch.Заказ;
                var techCard = order?.ТехКарта;
                var currentStepId = batch.ТекущийШагID;
                var currentStep = techCard?.Шаги.FirstOrDefault(s => s.ID == currentStepId);
                var executions = batch.ВыполнениеШагов.ToDictionary(e => e.ШагТехКартыID);

                // Определяем статус текущего шага
                string stepStatus = "не начат";
                if (currentStepId.HasValue && executions.ContainsKey(currentStepId.Value))
                {
                    var exec = executions[currentStepId.Value];
                    if (exec.ВремяОкончания.HasValue) stepStatus = "завершен";
                    else if (exec.ВремяСтарта.HasValue) stepStatus = "выполняется";
                }

                bool hasWarnings = executions.Any(e => e.Value.Отклонение && !IsCriticalDeviation(e.Value));
                bool hasCritical = executions.Any(e => e.Value.Отклонение && IsCriticalDeviation(e.Value));

                result.Add(new ActiveBatchDto
                {
                    ID = batch.ID,
                    НомерПартии = batch.НомерПартии,
                    Продукт = order != null ? _context.Products.Find(order.ПродуктID)?.Наименование ?? "—" : "—",
                    Линия = "Линия 1", // можно брать из оборудования, для простоты статика
                    ТекущийШаг = currentStep?.НаименованиеШага ?? "—",
                    СтатусПартии = batch.Статус ?? "—",
                    СтатусШага = stepStatus,
                    ЕстьПредупреждения = hasWarnings,
                    ЕстьКритическиеОтклонения = hasCritical
                });
            }
            return result;
        }

        private bool IsCriticalDeviation(BatchStepExecution exec)
        {
            var plan = exec.ШагТехКарты;
            if (plan == null) return false;
            // критическое отклонение: температура >5°, давление >0.5, длительность >30 мин
            return (exec.ФактТемпература.HasValue && Math.Abs(exec.ФактТемпература.Value - (plan.ПланТемпература ?? 0)) > 5) ||
                   (exec.ФактДавление.HasValue && Math.Abs(exec.ФактДавление.Value - (plan.ПланДавление ?? 0)) > 0.5m) ||
                   (exec.ФактДлительностьМинут.HasValue && Math.Abs(exec.ФактДлительностьМинут.Value - (plan.ПланДлительностьМинут ?? 0)) > 30);
        }

        public async Task<ProductionBatch?> GetByIdAsync(int id) =>
            await _context.ProductionBatches.Include(b => b.ВыполнениеШагов).FirstOrDefaultAsync(b => b.ID == id);

        public async Task<ProductionBatch?> StartAsync(int id)
        {
            var batch = await _context.ProductionBatches.FindAsync(id);
            if (batch == null) return null;
            batch.Статус = "выполняется";
            batch.ВремяСтарта = DateTime.Now;
            await _context.SaveChangesAsync();
            return batch;
        }

        public async Task<BatchStepExecution?> StartStepAsync(int stepId)
        {
            var step = await _context.BatchStepExecutions.FindAsync(stepId);
            if (step == null) return null;
            step.ВремяСтарта = DateTime.Now;
            await _context.SaveChangesAsync();
            return step;
        }

        public async Task<BatchStepExecution?> CompleteStepAsync(int stepId)
        {
            var step = await _context.BatchStepExecutions.FindAsync(stepId);
            if (step == null) return null;
            step.ВремяОкончания = DateTime.Now;
            await _context.SaveChangesAsync();
            return step;
        }

        public async Task<BatchStepExecution?> UpdateActualsAsync(int stepId, decimal? actualTemp, int? actualDuration, decimal? actualPressure, string? comment)
        {
            var step = await _context.BatchStepExecutions
                .Include(s => s.ШагТехКарты)
                .FirstOrDefaultAsync(s => s.ID == stepId);
            if (step == null) return null;

            step.ФактТемпература = actualTemp;
            step.ФактДлительностьМинут = actualDuration;
            step.ФактДавление = actualPressure;
            step.КомментарийОператора = comment;

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

            var order = await _context.ProductionOrders
                .Include(o => o.ТехКарта)
                    .ThenInclude(tc => tc!.Шаги)
                .FirstOrDefaultAsync(o => o.ID == batch.ЗаказID);
            if (order == null)
                throw new ArgumentException($"Заказ с ID {batch.ЗаказID} не найден");
            if (order.ТехКарта == null)
                throw new ArgumentException("К заказу не привязана технологическая карта");

            var steps = order.ТехКарта.Шаги.OrderBy(s => s.НомерШага).ToList();
            if (steps.Count == 0)
                throw new ArgumentException("У технологической карты нет шагов");

            _context.ProductionBatches.Add(batch);
            await _context.SaveChangesAsync();

            // Создаём записи выполнения шагов
            foreach (var step in steps)
            {
                var execution = new BatchStepExecution
                {
                    ПартияПроизводстваID = batch.ID,
                    ШагТехКартыID = step.ID,
                    Отклонение = false
                };
                _context.BatchStepExecutions.Add(execution);
            }
            await _context.SaveChangesAsync();

            // Устанавливаем первый шаг как текущий
            batch.ТекущийШагID = steps.First().ID;
            await _context.SaveChangesAsync();

            return batch;
        }

        public async Task<object> GetProgramAsync(int batchId)
        {
            var batch = await _context.ProductionBatches
                .Include(b => b.Заказ)
                    .ThenInclude(o => o!.ТехКарта)
                        .ThenInclude(tc => tc!.Шаги)
                .Include(b => b.ВыполнениеШагов)
                .FirstOrDefaultAsync(b => b.ID == batchId);
            if (batch == null) return null;

            var techCard = batch.Заказ?.ТехКарта;
            if (techCard == null) return null;

            var steps = techCard.Шаги.OrderBy(s => s.НомерШага).ToList();
            var executions = batch.ВыполнениеШагов.ToDictionary(e => e.ШагТехКартыID);

            var program = steps.Select(step => new
            {
                step.ID,
                step.НомерШага,
                step.НаименованиеШага,
                step.ПланТемпература,
                step.ПланДлительностьМинут,
                step.ПланДавление,
                step.Обязательный,
                step.Инструкция,
                СтатусВыполнения = executions.ContainsKey(step.ID)
                    ? (executions[step.ID].ВремяОкончания.HasValue ? "завершен" :
                       executions[step.ID].ВремяСтарта.HasValue ? "выполняется" : "не начат")
                    : "не начат",
                ФактТемпература = executions.ContainsKey(step.ID) ? executions[step.ID].ФактТемпература : null,
                ФактДлительностьМинут = executions.ContainsKey(step.ID) ? executions[step.ID].ФактДлительностьМинут : null,
                ФактДавление = executions.ContainsKey(step.ID) ? executions[step.ID].ФактДавление : null,
                Отклонение = executions.ContainsKey(step.ID) && executions[step.ID].Отклонение,
                ВыполнениеID = executions.ContainsKey(step.ID) ? executions[step.ID].ID : (int?)null
            }).ToList();

            return new { batch, Program = program };
        }

        public async Task<List<EventLog>> GetEventsAsync(int batchId)
        {
            return await _context.EventLogs
                .Where(e => e.ПартияПроизводстваID == batchId)
                .OrderByDescending(e => e.ВремяСобытия)
                .ToListAsync();
        }

        public async Task<bool> ReportIssueAsync(int batchId, string message, int operatorId)
        {
            var batch = await _context.ProductionBatches.FindAsync(batchId);
            if (batch == null) return false;

            var eventLog = new EventLog
            {
                ПартияПроизводстваID = batchId,
                ТипСобытия = "проблема",
                ВремяСобытия = DateTime.Now,
                Описание = message,
                Важность = "критическое",
                СоздалID = operatorId
            };
            _context.EventLogs.Add(eventLog);
            await _context.SaveChangesAsync();
            return true;
        }
        // AgroControl.API/Services/BatchService.cs
        public async Task<List<EventLog>> GetAllIssuesAsync()
        {
            return await _context.EventLogs
                .Where(e => e.ТипСобытия == "проблема" || e.Важность == "критическое")
                .Include(e => e.Создал)
                .OrderByDescending(e => e.ВремяСобытия)
                .ToListAsync();
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var batch = await _context.ProductionBatches
                .Include(b => b.ВыполнениеШагов)
                .FirstOrDefaultAsync(b => b.ID == id);
            if (batch == null) return false;

            // Сначала удаляем все выполнения шагов
            _context.BatchStepExecutions.RemoveRange(batch.ВыполнениеШагов);
            _context.ProductionBatches.Remove(batch);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}