using SIGEBI.Business.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIGEBI.Web.Services
{
    public class CatalogoService : ICatalogoService
    {
        private readonly IRecursosApi _recursosApi;
        private readonly IValoracionesApi _valoracionesApi;
        private readonly IListaDeseosApi _listaDeseosApi;
        private readonly IPrestamosApi _prestamosApi;

        public CatalogoService(
            IRecursosApi recursosApi, 
            IValoracionesApi valoracionesApi, 
            IListaDeseosApi listaDeseosApi, 
            IPrestamosApi prestamosApi)
        {
            _recursosApi = recursosApi;
            _valoracionesApi = valoracionesApi;
            _listaDeseosApi = listaDeseosApi;
            _prestamosApi = prestamosApi;
        }

        public async Task<List<RecursoDetalleDTO>> GetRecursosAsync(string token, string? busqueda = null)
        {
            if (string.IsNullOrWhiteSpace(busqueda))
            {
                return await _recursosApi.GetRecursosAsync(token);
            }
            return await _recursosApi.BuscarRecursosAsync(busqueda, token);
        }

        public async Task<RecursoDetalleDTO> GetRecursoAsync(Guid id, string token)
        {
            return await _recursosApi.GetRecursoAsync(id, token);
        }

        public async Task<List<ValoracionDTO>> GetValoracionesAsync(Guid recursoId, string token)
        {
            return await _valoracionesApi.GetValoracionesAsync(recursoId, token);
        }

        public async Task ValorarAsync(Guid usuarioId, Guid recursoId, int calificacion, string comentario, string token)
        {
            var request = new ValoracionDTO
            {
                UsuarioId = usuarioId,
                RecursoId = recursoId,
                Calificacion = calificacion,
                Comentario = comentario
            };

            await _valoracionesApi.ValorarAsync(request, token);
        }

        public async Task EliminarValoracionAsync(Guid id, string token)
        {
            await _valoracionesApi.EliminarValoracionAsync(id, token);
        }

        public async Task AgregarAListaDeseosAsync(Guid usuarioId, Guid recursoId, string token)
        {
            await _listaDeseosApi.AgregarAListaDeseosAsync(usuarioId, recursoId, token);
        }

        public async Task SolicitarPrestamoAsync(Guid usuarioId, Guid recursoId, string token)
        {
            var request = new PrestamoRequestDTO { UsuarioId = usuarioId, RecursoId = recursoId };
            await _prestamosApi.SolicitarPrestamoAsync(request, token);
        }
    }
}
