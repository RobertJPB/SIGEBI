using Microsoft.EntityFrameworkCore;
using SIGEBI.Infrastructure.Persistance;
using SIGEBI.Infrastructure.IoC;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// 🔹 Registrar DbContext
builder.Services.AddDbContext<SIGEBIDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// 🔹 Registrar dependencias (Repositories + Services)
builder.Services.AddInfrastructure();


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
