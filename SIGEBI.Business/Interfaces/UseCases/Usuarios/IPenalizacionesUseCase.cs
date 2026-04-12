using SIGEBI.Business.DTOs;

namespace SIGEBI.Business.Interfaces.UseCases.Usuarios
{
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

