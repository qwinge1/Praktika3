using AgroControl.API.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace AgroControl.API.IntegrationTests
{
    public class CustomWebApplicationFactory
    {
        public HttpClient Client { get; private set; }
        private TestServer _server;

        public void Initialize()
        {
            var builder = new WebHostBuilder()
                .UseEnvironment("Testing")
                .ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    if (descriptor != null) services.Remove(descriptor);
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseInMemoryDatabase("AgroControlTestDb"));
                })
                .UseStartup<TestStartup>();

            _server = new TestServer(builder);
            Client = _server.CreateClient();
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            using var scope = _server.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // 1. Добавляем пользователя
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Password123!");
            context.Users.Add(new User
            {
                ИмяПользователя = "tech.ivanov",
                ХэшПароля = hashedPassword,
                ПолноеИмя = "Иванов Технолог",
                Роль = "technologist",
                Активен = true,
                ДатаСоздания = DateTime.UtcNow
            });

            // 2. Добавляем продукт
            var product = new Product { Код = "P001", Наименование = "Гербицид", Статус = "активен" };
            context.Products.Add(product);
            context.SaveChanges();

            // 3. Добавляем рецепт (необходим для заказа)
            var recipe = new Recipe
            {
                ПродуктID = product.ID,
                Версия = 1,
                Статус = "активна",
                ДатаСоздания = DateTime.Now
            };
            context.Recipes.Add(recipe);
            context.SaveChanges();

            // 4. Добавляем технологическую карту и шаги
            var techCard = new TechCard { ПродуктID = product.ID, Версия = 1, Статус = "активна", ДатаСоздания = DateTime.Now };
            context.TechCards.Add(techCard);
            context.SaveChanges();

            var step = new TechCardStep
            {
                ТехКартаID = techCard.ID,
                НомерШага = 1,
                НаименованиеШага = "Смешивание",
                ПланТемпература = 80,
                ПланДлительностьМинут = 30,
                Обязательный = true
            };
            context.TechCardSteps.Add(step);
            context.SaveChanges();

            // 5. Добавляем заказ (используем реальные ID рецепта и техкарты)
            var order = new ProductionOrder
            {
                НомерЗаказа = "ORD-001",
                ПродуктID = product.ID,
                РецептID = recipe.ID,
                ТехКартаID = techCard.ID,
                ПланКоличество_кг = 1000,
                Статус = "запланирован"
            };
            context.ProductionOrders.Add(order);
            context.SaveChanges();

            // 6. Добавляем партию
            var batch = new ProductionBatch
            {
                НомерПартии = "B-001",
                ЗаказID = order.ID,
                Статус = "запланирована"
            };
            context.ProductionBatches.Add(batch);
            context.SaveChanges();

            // 7. Создаём запись выполнения шага
            var execution = new BatchStepExecution
            {
                ПартияПроизводстваID = batch.ID,
                ШагТехКартыID = step.ID,
                Отклонение = false
            };
            context.BatchStepExecutions.Add(execution);
            context.SaveChanges();

            // 8. Устанавливаем текущий шаг партии
            batch.ТекущийШагID = step.ID;
            context.SaveChanges();
        }

        public void Dispose()
        {
            Client?.Dispose();
            _server?.Dispose();
        }
    }
}