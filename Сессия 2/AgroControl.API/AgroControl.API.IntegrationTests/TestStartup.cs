using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using AgroControl.API.Services;
using AgroControl.API.Controllers;
using System.Text.Json.Serialization;

namespace AgroControl.API.IntegrationTests
{
    public class TestStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Настройка контроллеров с теми же JSON-опциями, что и в основном приложении
            services.AddControllers()
                .AddApplicationPart(typeof(ProductsController).Assembly)
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null; // отключаем camelCase
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; // игнорируем циклы
                });

            // Регистрация всех сервисов
            services.AddScoped<AuthService>();
            services.AddScoped<BatchService>();
            services.AddScoped<QualityControlService>();
            services.AddScoped<ReferenceService>();
            services.AddScoped<RecipeService>();
            services.AddScoped<TechCardService>();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}