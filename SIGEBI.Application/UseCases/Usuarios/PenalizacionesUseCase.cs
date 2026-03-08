using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Business.Mappers;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.UseCases.Usuarios
{
    public class PenalizacionesUseCase
    {
        private readonly IPrestamoRepository _prestamoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPenalizacionRepository _penalizacionRepository;

        public PenalizacionesUseCase(
            IPrestamoRepository prestamoRepository,
            IUsuarioRepository usuarioRepository,
            IPenalizacionRepository penalizacionRepository)
        {
            _prestamoRepository = prestamoRepository;
            _usuarioRepository = usuarioRepository;
            _penalizacionRepository = penalizacionRepository;
        }

        public async Task AplicarPenalizacionesAsync()
        {
            var prestamosAtrasados = await _prestamoRepository.GetAtrasadosAsync();

            foreach (var prestamo in prestamosAtrasados)
            {
                int diasAtraso = (int)(DateTime.UtcNow - prestamo.FechaDevolucionEstimada).TotalDays;
                int diasPenalizacion = PenalizacionCalculator.CalcularDiasPenalizacion(
                    prestamo.FechaDevolucionEstimada, DateTime.UtcNow);
                string motivo = PenalizacionCalculator.ObtenerMotivo(diasAtraso);

                var penalizacion = new Penalizacion(prestamo.UsuarioId, motivo, diasPenalizacion, DateTime.UtcNow);
                await _penalizacionRepository.AddAsync(penalizacion);
            }
        }

        public async Task<IEnumerable<PenalizacionDTO>> ObtenerPenalizacionesPorUsuarioAsync(Guid usuarioId)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
            if (usuario == null)
                throw new InvalidOperationException("Usuario no encontrado.");

            var penalizaciones = await _penalizacionRepository.GetByUsuarioIdAsync(usuarioId);
            return penalizaciones.Select(PenalizacionMapper.ToDTO);
        }
    }
}