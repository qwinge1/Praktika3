using AgroControl.API.Models;
using AgroControl.API.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<BatchService>();
builder.Services.AddScoped<QualityControlService>();
builder.Services.AddScoped<ReferenceService>();
builder.Services.AddScoped<RecipeService>();
builder.Services.AddScoped<TechCardService>();



builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // отключает camelCase
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ”бираем JWT-аутентификацию полностью
// builder.Services.AddAuthentication... Ц удалено

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()); // добавл€ем CORS дл€ удобства
app.MapControllers();
app.Run();