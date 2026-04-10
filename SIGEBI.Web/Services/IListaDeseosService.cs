using SIGEBI.Business.DTOs;
using System;
using System.Threading.Tasks;

namespace SIGEBI.Web.Services
{
    public interface IListaDeseosService
    {
        Task<ListaDeseosDTO> GetListaDeseosAsync(Guid usuarioId, string token);
        Task QuitarDeListaDeseosAsync(Guid usuarioId, Guid recursoId, string token);
    }
}
