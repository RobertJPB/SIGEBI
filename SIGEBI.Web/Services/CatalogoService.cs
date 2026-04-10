using SIGEBI.Business.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIGEBI.Web.Services
{
    public class CatalogoService : ICatalogoService
    {
        private readonly ISigebiApi _api;

        public CatalogoService(ISigebiApi api)
        {
            _api = api;
        }

        public async Task<List<RecursoDetalleDTO>> GetRecursosAsync(string token, string? busqueda = null)
        {
            if (string.IsNullOrWhiteSpace(busqueda))
            {
                return await _api.GetRecursosAsync(token);
            }
            return await _api.BuscarRecursosAsync(busqueda, token);
        }

        public async Task<RecursoDetalleDTO> GetRecursoAsync(Guid id, string token)
        {
            return await _api.GetRecursoAsync(id, token);
        }

        public async Task<List<ValoracionDTO>> GetValoracionesAsync(Guid recursoId, string token)
        {
            return await _api.GetValoracionesAsync(recursoId, token);
        }

        public async Task ValorarAsync(Guid usuarioId, Guid recursoId, int calificacion, string comentario, string token)
        {
            var request = new
            {
                UsuarioId = usuarioId,
                RecursoId = recursoId,
                Calificacion = calificacion,
                Comentario = comentario
            };

            await _api.ValorarAsync(request, token);
        }

        public async Task EliminarValoracionAsync(Guid id, string token)
        {
            await _api.EliminarValoracionAsync(id, token);
        }

        public async Task AgregarAListaDeseosAsync(Guid usuarioId, Guid recursoId, string token)
        {
            await _api.AgregarAListaDeseosAsync(usuarioId, recursoId, token);
        }

        public async Task SolicitarPrestamoAsync(Guid usuarioId, Guid recursoId, string token)
        {
            var request = new { UsuarioId = usuarioId, RecursoId = recursoId };
            await _api.SolicitarPrestamoAsync(request, token);
        }
    }
}
