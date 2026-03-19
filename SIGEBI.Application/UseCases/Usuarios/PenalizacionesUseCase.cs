using SIGEBI.Business.DTOs;
using SIGEBI.Application.Interfaces;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Business.Mappers;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.UseCases.Usuarios
{
    // Identifica devoluciones tardías y aplica sanciones automáticas a los usuarios.
    public class PenalizacionesUseCase
    {
        private readonly IPrestamoRepository _prestamoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPenalizacionRepository _penalizacionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PenalizacionesUseCase(
            IPrestamoRepository prestamoRepository,
            IUsuarioRepository usuarioRepository,
            IPenalizacionRepository penalizacionRepository,
            IUnitOfWork unitOfWork)
        {
            _prestamoRepository = prestamoRepository;
            _usuarioRepository = usuarioRepository;
            _penalizacionRepository = penalizacionRepository;
            _unitOfWork = unitOfWork;
        }

        // Proceso por lotes que evalúa préstamos atrasados y genera las penalizaciones correspondientes.
        public async Task AplicarPenalizacionesAsync()
        {
            // Este metodo deberia correrse todos los dias con un cronjob o algo asi
            var prestamosAtrasados = await _prestamoRepository.GetAtrasadosAsync();

            foreach (var prestamo in prestamosAtrasados)
            {
                int diasAtraso = (int)(DateTime.UtcNow - prestamo.FechaDevolucionEstimada).TotalDays;
                
                // calculamos cuantos dias le tocan de castigo
                int diasPenalizacion = PenalizacionCalculator.CalcularDiasPenalizacion(
                    prestamo.FechaDevolucionEstimada, DateTime.UtcNow);
                string motivo = PenalizacionCalculator.ObtenerMotivo(diasAtraso);

                var penalizacion = new Penalizacion(prestamo.UsuarioId, motivo, diasPenalizacion, DateTime.UtcNow);
                await _penalizacionRepository.AddAsync(penalizacion);
            }

            // guardamos todas las multas de una
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<PenalizacionDTO>> ObtenerPenalizacionesPorUsuarioAsync(Guid usuarioId)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId)
                ?? throw new InvalidOperationException("Usuario no encontrado.");

            var penalizaciones = await _penalizacionRepository.GetByUsuarioIdAsync(usuarioId);
            return penalizaciones.Select(PenalizacionMapper.ToDTO);
        }

        // Elimina permanentemente una sanción del sistema (hard delete).
        // Debe ser usado solo para corregir errores administrativos.
        public async Task EliminarPenalizacionAsync(Guid id)
        {
            var penalizacion = await _penalizacionRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("La penalización no existe.");

            _penalizacionRepository.Delete(penalizacion);
            await _unitOfWork.SaveChangesAsync();
        }

        // Resuelve manualmente una sanción antes de que expire su tiempo natural.
        // Útil si el usuario paga una multa o existe una justificación válida.
        public async Task FinalizarPenalizacionAsync(Guid id)
        {
            var penalizacion = await _penalizacionRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("La penalización no existe.");

            penalizacion.Finalizar(DateTime.UtcNow);
            _penalizacionRepository.Update(penalizacion);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}