using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using SIGEBI.Business.DTOs;

namespace SIGEBI.Services
{
    public interface INotificacionesApi
    {
        [Get("/api/Notificaciones/usuario/{usuarioId}")]
        Task<List<NotificacionDTO>> GetNotificacionesAsync(Guid usuarioId);

        [Delete("/api/Notificaciones/{id}")]
        Task EliminarNotificacionAsync(Guid id);

        [Put("/api/Notificaciones/usuario/{id}/leida")]
        Task MarcarNotificacionLeidaAsync(Guid id);

        [Put("/api/Notificaciones/usuario/{usuarioId}/leidas")]
        Task MarcarTodasComoLeidasAsync(Guid usuarioId);

        [Get("/api/Notificaciones/usuario/{usuarioId}/count")]
        Task<int> GetCantPendientesAsync(Guid usuarioId);
    }

    public interface IAuditoriaApi
    {
        [Get("/api/Auditoria")]
        Task<List<AuditoriaDTO>> GetAuditoriasAsync();
    }
}
