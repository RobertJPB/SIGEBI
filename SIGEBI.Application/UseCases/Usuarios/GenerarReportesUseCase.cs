using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Business.Mappers;

namespace SIGEBI.Business.UseCases.Usuarios
{
    // Principio SOLID (ISP - Segregación de Interfaces):
    // Solo dependemos de las interfaces estrictamente necesarias para los reportes 
    // (IPrestamoRepository, IUsuarioRepository, etc.) en lugar de un "IGestorCentral" gigante.
    public class GenerarReportesUseCase
    {
        private readonly IPrestamoRepository _prestamoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IRecursoRepository _recursoRepository;
        private readonly IPenalizacionRepository _penalizacionRepository;

        public GenerarReportesUseCase(
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

        public async Task<ReporteDTO> GenerarReporteGeneralAsync()
        {
            // Contamos los totales para el dashboard del admin
            var prestamos = await _prestamoRepository.GetAllAsync();
            var usuarios = await _usuarioRepository.GetAllAsync();
            var recursos = await _recursoRepository.GetAllAsync();
            var penalizaciones = await _penalizacionRepository.GetAllAsync();

            return new ReporteDTO
            {
                TotalPrestamos = prestamos.Count(),
                TotalUsuarios = usuarios.Count(),
                TotalRecursos = recursos.Count(),
                TotalPenalizaciones = penalizaciones.Count(),
                PrestamosAtrasados = prestamos.Count(p =>
                    p.EstadoActual == Domain.Enums.Biblioteca.EstadoPrestamo.Atrasado),
                FechaGeneracion = DateTime.UtcNow
            };
        }

        public async Task<IEnumerable<PrestamoResponseDTO>> ObtenerPrestamosPorPeriodoAsync(
            DateTime fechaInicio, DateTime fechaFin)
        {
            var prestamos = await _prestamoRepository.GetAllAsync();
            return prestamos
                .Where(p => p.FechaInicio >= fechaInicio && p.FechaInicio <= fechaFin)
                .Select(PrestamoMapper.ToDTO);
        }
    }
}