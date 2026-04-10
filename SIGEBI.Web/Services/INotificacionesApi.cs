using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using SIGEBI.Business.DTOs;

namespace SIGEBI.Web.Services
{
    public interface INotificacionesApi
    {
        [Get("/api/Notificaciones")]
        Task<List<NotificacionDTO>> GetAllNotificacionesAsync([Header("Authorization")] string token);

        [Get("/api/Notificaciones/usuario/{usuarioId}")]
        Task<List<NotificacionDTO>> GetNotificacionesByUsuarioAsync(Guid usuarioId, [Header("Authorization")] string token);

        [Put("/api/Notificaciones/{id}/leida")]
        Task MarcarLeidaAsync(Guid id, [Header("Authorization")] string token);

        [Put("/api/Notificaciones/usuario/{usuarioId}/leidas")]
        Task MarcarTodasLeidasAsync(Guid usuarioId, [Header("Authorization")] string token);

        [Get("/api/Notificaciones/usuario/{usuarioId}/count")]
        Task<int> GetUnreadCountAsync(Guid usuarioId, [Header("Authorization")] string token);
    }
}
