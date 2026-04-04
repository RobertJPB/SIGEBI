using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.Interfaces.UseCases.Usuarios;
using SIGEBI.Business.Mappers;

namespace SIGEBI.Business.UseCases.Usuarios
{
    // Consolida información estadística de múltiples entidades para fines administrativos.
    public class GenerarReportesUseCase : IGenerarReportesUseCase
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

        // Genera un resumen ejecutivo con métricas clave para el dashboard principal.
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

        // Filtra los préstamos realizados dentro de un intervalo temporal específico.
        public async Task<IEnumerable<PrestamoResponseDTO>> ObtenerPrestamosPorPeriodoAsync(
            DateTime fechaInicio, DateTime fechaFin)
        {
            var prestamos = await _prestamoRepository.GetAllAsync();
            return prestamos
                .Where(p => p.FechaInicio >= fechaInicio && p.FechaInicio <= fechaFin)
                .Select(PrestamoMapper.ToDTO);
        }

        // Identifica a los usuarios con mayor historial de sanciones para seguimiento administrativo.
        public async Task<IEnumerable<object>> ObtenerUsuariosMasPenalizadosAsync(int top = 5)
        {
            var penalizaciones = await _penalizacionRepository.GetAllAsync();
            return penalizaciones
                .GroupBy(p => p.UsuarioId)
                .Select(g => new
                {
                    UsuarioId = g.Key,
                    Nombre = g.First().Usuario?.Nombre ?? "N/A",
                    CantidadSanciones = g.Count()
                })
                .OrderByDescending(x => x.CantidadSanciones)
                .Take(top)
                .ToList();
        }

        // Devuelve el listado consolidado de todas las sanciones vigentes en el sistema.
        public async Task<IEnumerable<PenalizacionDTO>> ObtenerPenalizacionesActivasAsync()
        {
            var penalizaciones = await _penalizacionRepository.GetActivasAsync();
            return penalizaciones.Select(PenalizacionMapper.ToDTO);
        }
    }
}
