using SIGEBI.Business.DTOs;
using System;
using System.Threading.Tasks;

namespace SIGEBI.Web.Services
{
    public class ListaDeseosService : IListaDeseosService
    {
        private readonly ISigebiApi _api;

        public ListaDeseosService(ISigebiApi api)
        {
            _api = api;
        }

        public async Task<ListaDeseosDTO> GetListaDeseosAsync(Guid usuarioId, string token)
        {
            return await _api.GetListaDeseosAsync(usuarioId, token);
        }

        public async Task QuitarDeListaDeseosAsync(Guid usuarioId, Guid recursoId, string token)
        {
            await _api.QuitarDeListaDeseosAsync(usuarioId, recursoId, token);
        }
    }
}
