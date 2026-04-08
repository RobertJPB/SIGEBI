using Microsoft.AspNetCore.Authentication.Cookies;
using SIGEBI.Web.Services;
using Refit;

// Configuracion de la aplicacion web (MVC)

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Registro del Servicio de API con Refit (Cumpliendo Punto 2 del Documento)
builder.Services.AddRefitClient<ISigebiApi>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri("https://localhost:7047/");
    });

// HttpClient manual para controladores que no usan Refit directamente
builder.Services.AddHttpClient("SIGEBIAPI", c =>
{
    c.BaseAddress = new Uri("https://localhost:7047/");
});

// Configuración de sesiones para persistir el token JWT en el cliente
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// Configuración de Autenticación basada en cookies para el sitio web
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login"; // Redirección si no está autenticado
        options.AccessDeniedPath = "/Auth/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

// Pipeline de la peticion
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Configuración de la ruta por defecto (Catalogo/Index)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Catalogo}/{action=Index}/{id?}");

app.Run();
