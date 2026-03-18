using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using SIGEBI.API.Middleware;
using SIGEBI.Business.Interfaces.Services;
using SIGEBI.Infrastructure.Persistance;
using SIGEBI.Infrastructure.IoC;
using SIGEBI.Infrastructure.Services;
using System.Text;

// ── CONFIGURACIÓN DE LA APLICACIÓN (API) ──
var builder = WebApplication.CreateBuilder(args);

// Registro de controladores y configuración de JSON para evitar ciclos en las relaciones de las entidades
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Configuración de Swagger para la documentación de la API y pruebas de endpoints
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Configuración de seguridad Bearer (JWT) en Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Ingresa: Bearer {token}"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configuración del Contexto de Base de Datos (Entity Framework Core con SQL Server)
builder.Services.AddDbContext<SIGEBIDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Inyección de dependencias de las capas de infraestructura y servicios
builder.Services.AddInfrastructure();
builder.Services.AddScoped<IJwtService, JwtService>();

// Configuración de Autenticación basada en JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("WebPolicy", policy =>
    {
        policy.WithOrigins(
                builder.Configuration["AllowedOrigins:Web"]!)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ── PIPELINE DE LA PETICIÓN (MIDDLEWARE) ──
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware personalizado para la gestión global de excepciones
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseCors("WebPolicy");

// Configuración de archivos estáticos para servir imágenes
var imagenesPath = Path.Combine(builder.Environment.ContentRootPath, "imagenes");
Directory.CreateDirectory(imagenesPath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imagenesPath),
    RequestPath = "/imagenes"
});

// Autenticación y Autorización en el orden correcto
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
