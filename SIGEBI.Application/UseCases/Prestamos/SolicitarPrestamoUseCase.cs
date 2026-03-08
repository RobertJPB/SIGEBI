using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Business.Mappers;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.UseCases.Prestamos
{
    public class SolicitarPrestamoUseCase
    {
        private readonly IPrestamoRepository _prestamoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IRecursoRepository _recursoRepository;
        private readonly IPenalizacionRepository _penalizacionRepository;

        public SolicitarPrestamoUseCase(
            IPrestamoRepository prestamoRepository,
            IUsuarioRepository usuarioRepository,
            IRecursoRepository recursoRepository,
            IPenalizacionRepository penalizacionRepository)
        {
            _prestamoRepository = prestamoRepository;
            _usuarioRepository = usuarioRepository;
            _recursoRepository = recursoRepository;
            _penalizacionRepository = penalizacionRepository;
        }

        public async Task<PrestamoResponseDTO> EjecutarAsync(Guid usuarioId, Guid recursoId)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId)
                ?? throw new InvalidOperationException("Usuario no encontrado.");

            var recurso = await _recursoRepository.GetByIdAsync(recursoId)
                ?? throw new InvalidOperationException("Recurso no encontrado.");

            var prestamosActivos = await _prestamoRepository.GetActivosByUsuarioIdAsync(usuarioId);
            var penalizaciones = await _penalizacionRepository.GetByUsuarioIdAsync(usuarioId);

            PrestamoPolicy.ValidarPrestamo(usuario, recurso, prestamosActivos, penalizaciones);

            int diasPlazo = PrestamoPolicy.ObtenerDiasPlazo(usuario);
            var prestamo = new Prestamo(usuarioId, recursoId, diasPlazo, DateTime.UtcNow);

            recurso.DisminuirStock();
            await _prestamoRepository.AddAsync(prestamo);

            return PrestamoMapper.ToDTO(prestamo);
        }
    }
}