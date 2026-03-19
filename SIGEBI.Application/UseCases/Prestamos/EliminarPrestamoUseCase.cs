using SIGEBI.Application.Interfaces;
using SIGEBI.Business.Interfaces.Persistance;

namespace SIGEBI.Business.UseCases.Prestamos
{
    // Caso de uso para eliminar físicamente un registro de préstamo.
    // Solo debe ser usado para corrección de errores administrativos.
    public class EliminarPrestamoUseCase
    {
        private readonly IPrestamoRepository _prestamoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EliminarPrestamoUseCase(
            IPrestamoRepository prestamoRepository,
            IUnitOfWork unitOfWork)
        {
            _prestamoRepository = prestamoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task EjecutarAsync(Guid id)
        {
            var prestamo = await _prestamoRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Préstamo no encontrado.");

            _prestamoRepository.Delete(prestamo);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
