using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.Interfaces.Services
{
    public interface IPrestamoService
    {
        Task<Prestamo> SolicitarPrestamoAsync(Guid usuarioId, Guid recursoId);
        Task DevolverPrestamoAsync(Guid prestamoId);
        Task<IEnumerable<Prestamo>> GetPrestamosActivosByUsuarioAsync(Guid usuarioId);
    }
}