using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using SIGEBI.API.Middleware;
using SIGEBI.Business.Interfaces.Services;
using SIGEBI.Infrastructure.Persistence;
using SIGEBI.Infrastructure.Persistence.Interceptors;
using SIGEBI.Business.IoC;
using SIGEBI.Infrastructure.IoC;
using SIGEBI.Infrastructure.Services;
using System.Text;

// configuracion de la aplicacion API
var builder = WebApplication.CreateBuilder(args);

// Inyección de dependencias por capas
builder.Services.AddBusiness();
builder.Services.AddInfrastructure();

// Registro de controladores y configuración de JSON para evitar ciclos en las relaciones de las entidades
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMemoryCache();
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
builder.Services.AddDbContext<SIGEBIDbContext>((sp, options) =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure())
    .AddInterceptors(sp.GetRequiredService<AuditoriaInterceptor>()));
 

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

// pipeline de la peticion middleware 
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

// Bloqueo inmediato de usuarios no activos (Suspendidos/Bloqueados)
app.UseMiddleware<UserStatusMiddleware>();

app.MapControllers();
app.Run();
