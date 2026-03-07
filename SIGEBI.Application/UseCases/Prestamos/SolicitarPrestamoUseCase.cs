using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.UseCases.Prestamos
{
    public class SolicitarPrestamoUseCase
    {
        private readonly IPrestamoRepository _prestamoRepository;

        public SolicitarPrestamoUseCase(IPrestamoRepository prestamoRepository)
        {
            _prestamoRepository = prestamoRepository;
        }

        public async Task<Guid> Ejecutar(Guid usuarioId, Guid recursoId)
        {
            var prestamo = new Prestamo(
                usuarioId,
                recursoId,
                7,
                DateTime.Now
            );

            await _prestamoRepository.AddAsync(prestamo);

            return prestamo.Id;
        }
    }
}