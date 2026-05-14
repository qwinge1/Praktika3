using Microsoft.EntityFrameworkCore;
using AgroControl.API.Models;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Явное указание имён таблиц (русские названия)
            modelBuilder.Entity<LabTest>().ToTable("ЛабораторныеИспытания");
            modelBuilder.Entity<RawMaterialBatch>().ToTable("ПартииСырья");
            modelBuilder.Entity<ProductionBatch>().ToTable("ПроизводственныеПартии");
            modelBuilder.Entity<ProductionOrder>().ToTable("ПроизводственныеЗаказы");
            modelBuilder.Entity<Recipe>().ToTable("Рецептуры");
            modelBuilder.Entity<RecipeComponent>().ToTable("СоставРецептуры");
            modelBuilder.Entity<TechCard>().ToTable("ТехКарты");
            modelBuilder.Entity<TechCardStep>().ToTable("ШагиТехКарты");
            modelBuilder.Entity<User>().ToTable("Пользователи");
            modelBuilder.Entity<Product>().ToTable("Продукция");
            modelBuilder.Entity<RawMaterial>().ToTable("Сырье");
            modelBuilder.Entity<BatchStepExecution>().ToTable("ВыполнениеШаговПартии");
            modelBuilder.Entity<EventLog>().ToTable("ЖурналСобытий");
            modelBuilder.Entity<Equipment>().ToTable("Оборудование");

            // Игнорировать вычисляемые поля в RawMaterialBatch
            modelBuilder.Entity<RawMaterialBatch>()
                .Ignore(r => r.HasTest)
                .Ignore(r => r.LastTestDate);

            // Настройка связи для RawMaterialBatch -> RawMaterial
            modelBuilder.Entity<RawMaterialBatch>()
                .HasOne(r => r.Сырье)
                .WithMany()
                .HasForeignKey(r => r.СырьеID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<LabTest>()
    .HasOne(t => t.Исполнитель)
    .WithMany()
    .HasForeignKey(t => t.ИсполнительID);
        }
    }
}