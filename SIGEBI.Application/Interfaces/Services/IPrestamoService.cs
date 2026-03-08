using SIGEBI.Business.DTOs;

namespace SIGEBI.Business.Interfaces.Services
{
    public interface IPrestamoService
    {
        Task<PrestamoResponseDTO> SolicitarPrestamoAsync(Guid usuarioId, Guid recursoId);
        Task DevolverPrestamoAsync(Guid prestamoId);
        Task<IEnumerable<PrestamoResponseDTO>> GetPrestamosActivosByUsuarioAsync(Guid usuarioId);
    }
}