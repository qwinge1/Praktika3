using AgroControl.API.Models;

namespace AgroControl.API.Services
{
    public class QualityControlService
    {
        private readonly AppDbContext _context;
        public QualityControlService(AppDbContext context) => _context = context;

        public async Task<LabTest> CreateTestAsync(LabTest test)
        {
            // Соблюдаем ограничение CHECK: только одно поле должно быть не null
            if (test.ПартияПроизводстваID == null && test.ПартияСырьяID == null)
                throw new ArgumentException("Укажите либо партию производства, либо партию сырья");
            if (test.ПартияПроизводстваID != null && test.ПартияСырьяID != null)
                throw new ArgumentException("Нельзя указывать обе партии одновременно");

            _context.LabTests.Add(test);
            await _context.SaveChangesAsync();
            return test;
        }

        public async Task<bool> MakeDecisionAsync(int batchId, string decision, string? comment)
        {
            var batch = await _context.ProductionBatches.FindAsync(batchId);
            if (batch == null) return false;
            batch.Статус = decision == "одобрено" ? "завершена" : "заблокирована";
            await _context.SaveChangesAsync();
            return true;
        }
    }
}