using SIGEBI.Business.DTOs;

namespace SIGEBI.Business.Interfaces.UseCases.Prestamos
{
    public interface IConsultarPrestamoUseCase
    {
        Task<IEnumerable<PrestamoResponseDTO>> ObtenerTodosAsync();
        Task<IEnumerable<PrestamoResponseDTO>> ObtenerPorUsuarioAsync(Guid usuarioId);
        Task<IEnumerable<PrestamoResponseDTO>> ObtenerActivosPorUsuarioAsync(Guid usuarioId);
        Task<IEnumerable<PrestamoResponseDTO>> ObtenerAtrasadosAsync();
    }
}

