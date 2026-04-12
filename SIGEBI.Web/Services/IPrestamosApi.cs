using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using SIGEBI.Business.DTOs;

namespace SIGEBI.Web.Services
{
    public interface IPrestamosApi
    {
        [Get("/api/Prestamos/usuario/{usuarioId}")]
        Task<List<PrestamoResponseDTO>> GetPrestamosByUsuarioAsync(Guid usuarioId, [Header("Authorization")] string token);

        [Post("/api/Prestamos")]
        Task SolicitarPrestamoAsync([Body] PrestamoRequestDTO request, [Header("Authorization")] string token);
    }
}
