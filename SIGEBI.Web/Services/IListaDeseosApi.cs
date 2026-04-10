using System;
using System.Threading.Tasks;
using Refit;
using SIGEBI.Business.DTOs;

namespace SIGEBI.Web.Services
{
    public interface IListaDeseosApi
    {
        [Get("/api/ListaDeseos/usuario/{usuarioId}")]
        Task<ListaDeseosDTO> GetListaDeseosAsync(Guid usuarioId, [Header("Authorization")] string token);

        [Post("/api/ListaDeseos/usuario/{usuarioId}/recurso/{recursoId}")]
        Task AgregarAListaDeseosAsync(Guid usuarioId, Guid recursoId, [Header("Authorization")] string token);

        [Delete("/api/ListaDeseos/usuario/{usuarioId}/recurso/{recursoId}")]
        Task QuitarDeListaDeseosAsync(Guid usuarioId, Guid recursoId, [Header("Authorization")] string token);
    }
}
