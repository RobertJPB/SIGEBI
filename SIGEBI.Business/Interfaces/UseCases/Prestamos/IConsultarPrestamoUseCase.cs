using SIGEBI.Business.DTOs;

namespace SIGEBI.Business.Interfaces.UseCases.Prestamos
{
    /// <summary>
    /// Contrato para las consultas de préstamos: listado global, por usuario y atrasados.
    /// </summary>
    public interface IConsultarPrestamoUseCase
    {
        Task<IEnumerable<PrestamoResponseDTO>> ObtenerTodosAsync();
        Task<IEnumerable<PrestamoResponseDTO>> ObtenerPorUsuarioAsync(Guid usuarioId);
        Task<IEnumerable<PrestamoResponseDTO>> ObtenerActivosPorUsuarioAsync(Guid usuarioId);
        Task<IEnumerable<PrestamoResponseDTO>> ObtenerAtrasadosAsync();
    }
}
