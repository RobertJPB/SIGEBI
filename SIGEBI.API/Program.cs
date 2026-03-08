using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SIGEBI.API.Middleware;
using SIGEBI.Infrastructure.Persistance;
using SIGEBI.Infrastructure.IoC;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Servicios base
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrar DbContext
builder.Services.AddDbContext<SIGEBIDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar dependencias (Repositorios + Servicios + Casos de uso)
builder.Services.AddInfrastructure();

// Configurar autenticacion con JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,           // Validar quien emite el token
            ValidateAudience = true,         // Validar para quien es el token
            ValidateLifetime = true,         // Validar que el token no haya expirado
            ValidateIssuerSigningKey = true, // Validar la firma del token
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

var app = builder.Build();

// Configurar el pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>(); // Manejo global de excepciones
app.UseHttpsRedirection();
app.UseAuthentication(); // Primero autenticacion
app.UseAuthorization();  // Luego autorizacion
app.MapControllers();
app.Run();