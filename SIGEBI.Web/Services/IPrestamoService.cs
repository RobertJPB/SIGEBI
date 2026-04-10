using SIGEBI.Business.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIGEBI.Web.Services
{
    public interface IPrestamoService
    {
        Task<List<PrestamoResponseDTO>> GetPrestamosByUsuarioAsync(Guid usuarioId, string token);
    }
}
