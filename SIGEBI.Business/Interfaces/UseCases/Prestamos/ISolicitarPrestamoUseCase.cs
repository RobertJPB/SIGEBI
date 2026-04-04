using SIGEBI.Business.DTOs;

namespace SIGEBI.Business.Interfaces.UseCases.Prestamos
{
    /// <summary>
    /// Contrato para el caso de uso que orquesta la solicitud de un préstamo bibliográfico.
    /// Permite suplantar la implementación concreta en pruebas unitarias.
    /// </summary>
    public interface ISolicitarPrestamoUseCase
    {
        Task<PrestamoResponseDTO> EjecutarAsync(Guid usuarioId, Guid recursoId, DateTime? fechaDevolucionEstimada = null);
    }
}
