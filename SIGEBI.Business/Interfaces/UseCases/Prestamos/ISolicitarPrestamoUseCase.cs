using SIGEBI.Business.DTOs;

namespace SIGEBI.Business.Interfaces.UseCases.Prestamos
{
    public interface ISolicitarPrestamoUseCase
    {
        Task<PrestamoResponseDTO> EjecutarAsync(Guid usuarioId, Guid recursoId, DateTime? fechaDevolucionEstimada = null);
    }
}

