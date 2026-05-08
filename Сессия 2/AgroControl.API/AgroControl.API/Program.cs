using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.IdentityModel.Tokens;

using Microsoft.OpenApi.Models;

using System.Text;

using AgroControl.API.Models;

using AgroControl.API.Services;



var builder = WebApplication.CreateBuilder(args);



// Database

builder.Services.AddDbContext<AppDbContext>(options =>

    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



// JWT Authentication

var jwtKey = builder.Configuration["Jwt:Key"]!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)

    .AddJwtBearer(options =>

    {

        options.TokenValidationParameters = new TokenValidationParameters

        {

            ValidateIssuer = true,

            ValidateAudience = true,

            ValidateIssuerSigningKey = true,

            ValidIssuer = "AgroControl",

            ValidAudience = "AgroControl",

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))

        };

    });



// Services

builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<BatchService>();

builder.Services.AddScoped<QualityControlService>();

builder.Services.AddScoped<ReferenceService>();

builder.Services.AddScoped<RecipeService>();

builder.Services.AddScoped<TechCardService>();



// Controllers + JSON options

builder.Services.AddControllers()

    .AddJsonOptions(options =>

    {

        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;

        options.JsonSerializerOptions.PropertyNamingPolicy = null; // сохраняем русские имена свойств как в моделях

    });



// Swagger

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>

{

    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AgroControl API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme

    {

        Description = "JWT Authorization header. Example: \"Bearer {token}\"",

        Name = "Authorization",

        In = ParameterLocation.Header,

        Type = SecuritySchemeType.ApiKey,

        Scheme = "Bearer"

    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement

    {

        {

            new OpenApiSecurityScheme

            {

                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }

            },

            Array.Empty<string>()

        }

    });

});



var app = builder.Build();



if (app.Environment.IsDevelopment())

{

    app.UseSwagger();

    app.UseSwaggerUI();

}



app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();