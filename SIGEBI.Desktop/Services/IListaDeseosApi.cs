using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using SIGEBI.Business.DTOs;

namespace SIGEBI.Services
{
    public interface IListaDeseosApi
    {
        [Get("/api/ListaDeseos/usuario/{usuarioId}")]
        Task<ListaDeseosDTO> GetListaDeseosAsync(Guid usuarioId);

        [Post("/api/ListaDeseos/usuario/{usuarioId}/recurso/{recursoId}")]
        Task AgregarAListaDeseosAsync(Guid usuarioId, Guid recursoId);

        [Delete("/api/ListaDeseos/usuario/{usuarioId}/recurso/{recursoId}")]
        Task QuitarDeListaDeseosAsync(Guid usuarioId, Guid recursoId);
    }
}
