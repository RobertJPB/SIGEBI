using Microsoft.Extensions.Caching.Memory;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Domain.Enums.Seguridad;
using System.Security.Claims;

namespace SIGEBI.API.Middleware
{
    /// <summary>
    /// Middleware encargado de verificar en tiempo real si la cuenta del usuario sigue activa.
    /// Si un administrador suspende al usuario, este middleware rechazará cualquier petición posterior.
    /// </summary>
    public class UserStatusMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private const string CachePrefix = "UserStatus_";

        public UserStatusMiddleware(RequestDelegate next, IMemoryCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext context, IUsuarioRepository usuarioRepository)
        {
            var user = context.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (Guid.TryParse(userIdClaim, out var userId))
                {
                    var cacheKey = $"{CachePrefix}{userId}";

                    if (!_cache.TryGetValue(cacheKey, out EstadoUsuario estado))
                    {
                        var usuario = await usuarioRepository.GetByIdAsync(userId);
                        
                        if (usuario == null)
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            await context.Response.WriteAsync("Usuario no encontrado.");
                            return;
                        }

                        estado = usuario.Estado;

                        // Cachear el estado por 1 minuto para no saturar la BD
                        _cache.Set(cacheKey, estado, TimeSpan.FromMinutes(1));
                    }

                    if (estado != EstadoUsuario.Activo)
                    {
                        var usuario = await usuarioRepository.GetByIdAsync(userId);
                        
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";
                        
                        string? motivo = null;
                        DateTime? fechaFin = null;

                        if (estado == EstadoUsuario.Suspendido)
                        {
                            var penalizacion = usuario?.Penalizaciones?.FirstOrDefault(p => p.Estado == SIGEBI.Domain.Enums.Operacion.EstadoPenalizacion.Activa);
                            motivo = penalizacion?.Motivo;
                            fechaFin = penalizacion?.FechaFin;
                        }
                        else
                        {
                            motivo = usuario?.MotivoEstado;
                        }

                        var message = estado switch
                        {
                            EstadoUsuario.Suspendido => "Su acceso ha sido suspendido temporalmente por infracciones.",
                            EstadoUsuario.Bloqueado => "Su cuenta ha sido bloqueada permanentemente por faltas graves.",
                            EstadoUsuario.Inactivo => "Esta cuenta ha sido desactivada administrativamente.",
                            _ => "Acceso denegado por restricciones en su cuenta."
                        };

                        if (!string.IsNullOrWhiteSpace(motivo)) message += $" Razón: {motivo}.";
                        if (fechaFin.HasValue) message += $" Hasta: {fechaFin.Value:dd/MM/yyyy}.";

                        await context.Response.WriteAsJsonAsync(new { 
                            error = "CuentaRestringida", 
                            message = message,
                            estado = (int)estado,
                            motivo = motivo,
                            fechaFin = fechaFin
                        });
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
