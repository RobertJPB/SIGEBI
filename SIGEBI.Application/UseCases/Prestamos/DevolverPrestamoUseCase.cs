using SIGEBI.Business.Interfaces.Persistance;

namespace SIGEBI.Business.UseCases.Prestamos
{
    public class DevolverPrestamoUseCase
    {
        private readonly IPrestamoRepository _prestamoRepository;

        public DevolverPrestamoUseCase(IPrestamoRepository prestamoRepository)
        {
            _prestamoRepository = prestamoRepository;
        }

        public async Task Ejecutar(Guid prestamoId)
        {
            var prestamo = await _prestamoRepository.GetByIdAsync(prestamoId);

            if (prestamo == null)
                throw new Exception("Préstamo no encontrado");

            prestamo.Devolver(DateTime.UtcNow);

            _prestamoRepository.Update(prestamo);
        }
    }
}
