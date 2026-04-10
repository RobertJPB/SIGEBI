using SIGEBI.Business.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIGEBI.Web.Services
{
    public interface INotificacionService
    {
        Task<List<NotificacionDTO>> GetAllNotificacionesAsync(string token);
        Task<List<NotificacionDTO>> GetNotificacionesByUsuarioAsync(Guid usuarioId, string token);
        Task MarcarLeidaAsync(Guid id, string token);
        Task MarcarTodasLeidasAsync(Guid usuarioId, string token);
        Task<int> GetUnreadCountAsync(Guid usuarioId, string token);
    }
}
