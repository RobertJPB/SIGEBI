using SIGEBI.Business.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIGEBI.Web.Services
{
    public class PrestamoService : IPrestamoService
    {
        private readonly ISigebiApi _api;

        public PrestamoService(ISigebiApi api)
        {
            _api = api;
        }

        public async Task<List<PrestamoResponseDTO>> GetPrestamosByUsuarioAsync(Guid usuarioId, string token)
        {
            return await _api.GetPrestamosByUsuarioAsync(usuarioId, token);
        }
    }
}
