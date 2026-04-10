using SIGEBI.Domain.Enums.Auditoria;

namespace SIGEBI.Business.Interfaces.Common
{
    /// <summary>
    /// Servicio centralizado para el registro de eventos de auditoría de negocio.
    /// Permite capturar acciones que no necesariamente disparan cambios en la base de datos
    /// (ej. Logins, Accesos Denegados, Consultas Sensibles).
    /// </summary>
    public interface IAuditService
    {
        /// <summary>
        /// Registra un evento de auditoría de forma explícita.
        /// </summary>
        /// <param name="accion">Tipo de acción realizada.</param>
        /// <param name="tabla">Entidad o Módulo afectado.</param>
        /// <param name="detalle">Descripción narrativa del evento.</param>
        /// <param name="usuarioId">Opcional. ID del usuario si no se puede obtener del contexto.</param>
        Task LogActionAsync(TipoAccionAuditoria accion, string tabla, string detalle, Guid? usuarioId = null);

        /// <summary>
        /// Registra un evento de auditoría en segundo plano sin bloquear el flujo principal.
        /// Útil para eventos de login o accesos que no deben retardar la experiencia de usuario.
        /// </summary>
        void LogActionBackground(TipoAccionAuditoria accion, string tabla, string detalle, Guid? usuarioId = null);
    }
}
