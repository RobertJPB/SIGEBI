using SIGEBI.Business.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIGEBI.Web.Services
{
    public class NotificacionService : INotificacionService
    {
        private readonly INotificacionesApi _api;

        public NotificacionService(INotificacionesApi api)
        {
            _api = api;
        }

        public async Task<List<NotificacionDTO>> GetAllNotificacionesAsync(string token)
        {
            return await _api.GetAllNotificacionesAsync(token);
        }

        public async Task<List<NotificacionDTO>> GetNotificacionesByUsuarioAsync(Guid usuarioId, string token)
        {
            return await _api.GetNotificacionesByUsuarioAsync(usuarioId, token);
        }

        public async Task MarcarLeidaAsync(Guid id, string token)
        {
            await _api.MarcarLeidaAsync(id, token);
        }

        public async Task MarcarTodasLeidasAsync(Guid usuarioId, string token)
        {
            await _api.MarcarTodasLeidasAsync(usuarioId, token);
        }

        public async Task<int> GetUnreadCountAsync(Guid usuarioId, string token)
        {
            return await _api.GetUnreadCountAsync(usuarioId, token);
        }
    }
}
