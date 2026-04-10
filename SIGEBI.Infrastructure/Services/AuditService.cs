using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.Common;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Enums.Auditoria;
using System.Security.Claims;

namespace SIGEBI.Infrastructure.Services
{
    public class AuditService : global::SIGEBI.Business.Interfaces.Common.IAuditService
    {
        private readonly IAuditoriaRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceScopeFactory _scopeFactory;

        public AuditService(
            IAuditoriaRepository repository,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IServiceScopeFactory scopeFactory)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _scopeFactory = scopeFactory;
        }

        public async Task LogActionAsync(TipoAccionAuditoria accion, string tabla, string detalle, Guid? usuarioId = null)
        {
            var auditRecord = CrearRegistro(accion, tabla, detalle, usuarioId);

            await _repository.AddAsync(auditRecord);
            await _unitOfWork.SaveChangesAsync();
        }

        public void LogActionBackground(TipoAccionAuditoria accion, string tabla, string detalle, Guid? usuarioId = null)
        {
            // Capturamos los datos necesarios del contexto actual de inmediato antes de que la request termine o el contexto se elimine
            var auditRecord = CrearRegistro(accion, tabla, detalle, usuarioId);

            // Ejecutamos en segundo plano usando un nuevo scope para evitar que el DbContext original se cierre al terminar la request
            _ = Task.Run(async () =>
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<IAuditoriaRepository>();
                    var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    await repo.AddAsync(auditRecord);
                    await uow.SaveChangesAsync();
                }
                catch
                {
                    // En un sistema real, loguearíamos este fallo en un log de archivos local o servicio de monitoreo
                }
            });
        }

        private Auditoria CrearRegistro(TipoAccionAuditoria accion, string tabla, string detalle, Guid? usuarioId = null)
        {
            var actorId = usuarioId;

            if (!actorId.HasValue)
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                 ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;

                if (Guid.TryParse(userIdClaim, out var parsedId))
                {
                    actorId = parsedId;
                }
            }

            return new Auditoria(
                usuarioId: actorId,
                accion: accion,
                tablaAfectada: tabla,
                detalle: detalle,
                ipAddress: _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "::1",
                fechaRegistroUtc: DateTime.UtcNow
            );
        }
    }
}
