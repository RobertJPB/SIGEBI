using SIGEBI.Business.DTOs;

namespace SIGEBI.Business.Interfaces.UseCases.Usuarios
{
    /// <summary>
    /// Contrato para la gestión de penalizaciones automáticas y manuales por devoluciones tardías.
    /// </summary>
    public interface IPenalizacionesUseCase
    {
        Task AplicarPenalizacionesAsync();
        Task AplicarPenalizacionManualAsync(AplicarPenalizacionManualDTO dto);
        Task<IEnumerable<PenalizacionDTO>> ObtenerTodasLasPenalizacionesAsync();
        Task<IEnumerable<PenalizacionDTO>> ObtenerPenalizacionesPorUsuarioAsync(Guid usuarioId);
        Task EliminarPenalizacionAsync(Guid id);
        Task FinalizarPenalizacionAsync(Guid id);
    }
}
