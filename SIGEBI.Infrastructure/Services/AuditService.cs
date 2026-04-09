using Microsoft.AspNetCore.Http;
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

        public AuditService(
            IAuditoriaRepository repository,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogActionAsync(TipoAccionAuditoria accion, string tabla, string detalle, Guid? usuarioId = null)
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

            var auditRecord = new Auditoria(
                usuarioId: actorId,
                accion: accion,
                tablaAfectada: tabla,
                detalle: detalle,
                ipAddress: _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "::1",
                fechaRegistroUtc: DateTime.UtcNow
            );

            await _repository.AddAsync(auditRecord);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
