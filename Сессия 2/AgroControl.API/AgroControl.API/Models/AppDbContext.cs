using Microsoft.EntityFrameworkCore;

namespace AgroControl.API.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<RawMaterial> RawMaterials => Set<RawMaterial>();
        public DbSet<Recipe> Recipes => Set<Recipe>();
        public DbSet<RecipeComponent> RecipeComponents => Set<RecipeComponent>();
        public DbSet<TechCard> TechCards => Set<TechCard>();
        public DbSet<TechCardStep> TechCardSteps => Set<TechCardStep>();
        public DbSet<ProductionOrder> ProductionOrders => Set<ProductionOrder>();
        public DbSet<ProductionBatch> ProductionBatches => Set<ProductionBatch>();
        public DbSet<BatchStepExecution> BatchStepExecutions => Set<BatchStepExecution>();
        public DbSet<LabTest> LabTests => Set<LabTest>();
        public DbSet<RawMaterialBatch> RawMaterialBatches => Set<RawMaterialBatch>();
        public DbSet<EventLog> EventLogs => Set<EventLog>();
        public DbSet<Equipment> Equipment => Set<Equipment>();
    }
}