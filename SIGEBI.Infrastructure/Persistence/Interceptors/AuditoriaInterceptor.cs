using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Enums.Auditoria;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SIGEBI.Infrastructure.Persistence.Interceptors
{
    public class AuditoriaInterceptor : SaveChangesInterceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly Guid UsuarioIdSistema = Guid.Parse("00000000-0000-0000-0000-000000000001");

        public AuditoriaInterceptor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            if (eventData.Context == null) return result;

            GenerarAuditoria(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            if (eventData.Context == null) return new ValueTask<InterceptionResult<int>>(result);

            GenerarAuditoria(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void GenerarAuditoria(DbContext context)
        {
            var entries = context.ChangeTracker.Entries()
                .Where(e => (e.State == EntityState.Added || 
                             e.State == EntityState.Modified || 
                             e.State == EntityState.Deleted) && 
                             e.Entity is not Auditoria)
                .ToList();

            foreach (var entry in entries)
            {
                var action = entry.State switch
                {
                    EntityState.Added => TipoAccionAuditoria.Crear,
                    EntityState.Modified => TipoAccionAuditoria.Actualizar,
                    EntityState.Deleted => TipoAccionAuditoria.Eliminar,
                    _ => TipoAccionAuditoria.Actualizar
                };

                var tableName = entry.Entity.GetType().Name;
                var actorId = UsuarioIdSistema;

                // Intentar obtener el ID del usuario desde el contexto HTTP (JWT sub claim)
                var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                                ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;

                if (Guid.TryParse(userIdClaim, out var parsedId))
                {
                    actorId = parsedId;
                }
                else if (entry.Entity is Usuario u && entry.State == EntityState.Added)
                {
                    actorId = u.Id;
                }

                var detalle = GenerarDetalleDescriptivo(entry);

                var auditRecord = new Auditoria(
                    usuarioId: actorId,
                    accion: action,
                    tablaAfectada: tableName,
                    detalle: detalle,
                    ipAddress: _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "::1",
                    fechaRegistroUtc: DateTime.UtcNow
                );

                context.Set<Auditoria>().Add(auditRecord);
            }
        }

        private string GenerarDetalleDescriptivo(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
        {
            var entityName = entry.Entity.GetType().Name;
            
            if (entry.State == EntityState.Added)
            {
                var nombre = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "Nombre" || p.Metadata.Name == "Titulo")?.CurrentValue;
                return nombre != null ? $"Creado nuevo {entityName}: {nombre}" : $"Creación de registro en {entityName}";
            }

            if (entry.State == EntityState.Deleted)
            {
                return $"Eliminación permanente de registro en {entityName}";
            }

            if (entry.State == EntityState.Modified)
            {
                var cambios = entry.Properties
                    .Where(p => p.IsModified && !Equals(p.OriginalValue, p.CurrentValue))
                    .Select(p => $"{p.Metadata.Name}: '{p.OriginalValue ?? "null"}' -> '{p.CurrentValue ?? "null"}'")
                    .ToList();

                if (cambios.Any())
                {
                    var msg = $"Actualización en {entityName}. Cambios: {string.Join(" | ", cambios)}";
                    return msg.Length > 450 ? msg.Substring(0, 447) + "..." : msg;
                }
                
                return $"Actualización detectada en {entityName} (sin cambios aparentes en propiedades)";
            }

            return $"Acción sobre {entityName}";
        }
    }
}
