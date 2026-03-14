using SIGEBI.Application.Interfaces;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.UseCases.Prestamos
{
    // Principio SOLID (SRP y DIP):
    // SRP: Solo maneja el flujo de devolver un libro.
    // DIP: Depende de interfaces de repositorios para no acoplarse a la base de datos.
    public class DevolverPrestamoUseCase
    {
        private readonly IPrestamoRepository _prestamoRepository;
        private readonly IRecursoRepository _recursoRepository;
        private readonly IPenalizacionRepository _penalizacionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DevolverPrestamoUseCase(
            IPrestamoRepository prestamoRepository,
            IRecursoRepository recursoRepository,
            IPenalizacionRepository penalizacionRepository,
            IUnitOfWork unitOfWork)
        {
            _prestamoRepository = prestamoRepository;
            _recursoRepository = recursoRepository;
            _penalizacionRepository = penalizacionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task EjecutarAsync(Guid prestamoId)
        {
            var prestamo = await _prestamoRepository.GetByIdAsync(prestamoId)
                ?? throw new InvalidOperationException("Préstamo no encontrado.");

            var recurso = await _recursoRepository.GetByIdAsync(prestamo.RecursoId)
                ?? throw new InvalidOperationException("Recurso no encontrado.");

            prestamo.Devolver(DateTime.UtcNow);
            recurso.AumentarStock(); // el libro vuelve a estar disponible

            // Verificamos si lo entrego tarde para clavarle una multa
            if (PenalizacionCalculator.TienePenalizacion(prestamo.FechaDevolucionEstimada, DateTime.UtcNow))
            {
                int diasAtraso = (int)(DateTime.UtcNow - prestamo.FechaDevolucionEstimada).TotalDays;
                int diasPenalizacion = PenalizacionCalculator.CalcularDiasPenalizacion(
                    prestamo.FechaDevolucionEstimada, DateTime.UtcNow);
                
                string motivo = PenalizacionCalculator.ObtenerMotivo(diasAtraso);

                // TODO: ¿Deberíamos notificar por correo aquí también?
                var penalizacion = new Penalizacion(prestamo.UsuarioId, motivo, diasPenalizacion, DateTime.UtcNow);
                await _penalizacionRepository.AddAsync(penalizacion);
            }

            _prestamoRepository.Update(prestamo);
            _recursoRepository.Update(recurso);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}