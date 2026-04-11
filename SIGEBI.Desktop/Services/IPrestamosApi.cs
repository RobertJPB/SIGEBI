using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using SIGEBI.Business.DTOs;

namespace SIGEBI.Services
{
    public interface IPrestamosApi
    {
        [Get("/api/Prestamos")]
        Task<List<PrestamoResponseDTO>> GetPrestamosAsync();

        [Post("/api/Prestamos")]
        Task<PrestamoResponseDTO> SolicitarPrestamoAsync([Body] PrestamoRequestDTO request);

        [Put("/api/Prestamos/devolver/{prestamoId}")]
        Task DevolverPrestamoAsync(Guid prestamoId);

        [Delete("/api/Prestamos/{prestamoId}")]
        Task EliminarPrestamoAsync(Guid prestamoId);
    }

    public interface IPenalizacionesApi
    {
        [Get("/api/Penalizaciones")]
        Task<List<PenalizacionDTO>> GetPenalizacionesAsync();

        [Patch("/api/Penalizaciones/{id}/finalizar")]
        Task FinalizarPenalizacionAsync(Guid id);

        [Delete("/api/Penalizaciones/{id}")]
        Task EliminarPenalizacionAsync(Guid id);

        [Post("/api/Penalizaciones")]
        Task AplicarPenalizacionManualAsync([Body] AplicarPenalizacionManualDTO dto);

        [Post("/api/Penalizaciones/aplicar")]
        Task AplicarPenalizacionesAsync();
    }
}
