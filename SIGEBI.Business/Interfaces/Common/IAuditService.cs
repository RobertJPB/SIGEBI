using SIGEBI.Domain.Enums.Auditoria;

namespace SIGEBI.Business.Interfaces.Common
{
    public interface IAuditService
    {
        Task LogActionAsync(TipoAccionAuditoria accion, string tabla, string detalle, Guid? usuarioId = null);
        void LogActionBackground(TipoAccionAuditoria accion, string tabla, string detalle, Guid? usuarioId = null);
    }
}


