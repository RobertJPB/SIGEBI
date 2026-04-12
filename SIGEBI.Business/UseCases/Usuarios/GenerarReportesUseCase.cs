using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.Interfaces.UseCases.Usuarios;
using SIGEBI.Business.Mappers;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Enums.Biblioteca;
using SIGEBI.Business.Interfaces.Common;
using System.Text.Json;

namespace SIGEBI.Business.UseCases.Usuarios
{
    // Consolida información estadística de múltiples entidades para fines administrativos.
    public class GenerarReportesUseCase : IGenerarReportesUseCase
    {
        private readonly IPrestamoRepository _prestamoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IRecursoRepository _recursoRepository;
        private readonly IPenalizacionRepository _penalizacionRepository;
        private readonly IReporteRepository _reporteRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGuidGenerator _guidGenerator;

        public GenerarReportesUseCase(
            IPrestamoRepository prestamoRepository,
            IUsuarioRepository usuarioRepository,
            IRecursoRepository recursoRepository,
            IPenalizacionRepository penalizacionRepository,
            IReporteRepository reporteRepository,
            IUnitOfWork unitOfWork,
            IGuidGenerator guidGenerator)
        {
            _prestamoRepository = prestamoRepository;
            _usuarioRepository = usuarioRepository;
            _recursoRepository = recursoRepository;
            _penalizacionRepository = penalizacionRepository;
            _reporteRepository = reporteRepository;
            _unitOfWork = unitOfWork;
            _guidGenerator = guidGenerator;
        }

        // Genera un resumen ejecutivo con métricas clave para el dashboard principal.
        public async Task<ReporteDTO> GenerarReporteGeneralAsync()
        {
            var prestamos = await _prestamoRepository.GetAllAsync();
            var usuarios = await _usuarioRepository.GetAllAsync();
            var recursos = await _recursoRepository.GetAllAsync();
            var penalizaciones = await _penalizacionRepository.GetAllAsync();

            var dto = new ReporteDTO
            {
                TotalPrestamos = prestamos.Count(),
                TotalUsuarios = usuarios.Count(),
                TotalRecursos = recursos.Count(),
                TotalPenalizaciones = penalizaciones.Count(),
                PrestamosAtrasados = prestamos.Count(p =>
                    p.EstadoActual == Domain.Enums.Biblioteca.EstadoPrestamo.Atrasado),
                FechaGeneracion = DateTime.UtcNow
            };

            await GuardarReporteAsync(TipoReporte.General, "{}", JsonSerializer.Serialize(dto));

            return dto;
        }

        // Filtra los préstamos realizados dentro de un intervalo temporal específico.
        public async Task<IEnumerable<PrestamoResponseDTO>> ObtenerPrestamosPorPeriodoAsync(
            DateTime fechaInicio, DateTime fechaFin, Guid? usuarioId = null, Guid? recursoId = null)
        {
            var prestamos = await _prestamoRepository.GetAllAsync();
            var filtrados = prestamos
                .Where(p => p.FechaInicio >= fechaInicio && p.FechaInicio <= fechaFin);

            if (usuarioId.HasValue && usuarioId != Guid.Empty)
                filtrados = filtrados.Where(p => p.UsuarioId == usuarioId.Value);

            if (recursoId.HasValue && recursoId != Guid.Empty)
                filtrados = filtrados.Where(p => p.RecursoId == recursoId.Value);

            var resultado = filtrados.Select(PrestamoMapper.ToDTO).ToList();

            var parametros = new { FechaInicio = fechaInicio, FechaFin = fechaFin, UsuarioId = usuarioId, RecursoId = recursoId };
            await GuardarReporteAsync(TipoReporte.PrestamosPorPeriodo, JsonSerializer.Serialize(parametros), JsonSerializer.Serialize(resultado));

            return resultado;
        }

        // Identifica a los usuarios con mayor historial de sanciones para seguimiento administrativo.
        public async Task<IEnumerable<object>> ObtenerUsuariosMasPenalizadosAsync(int top = 5)
        {
            var penalizaciones = await _penalizacionRepository.GetAllAsync();
            var resultado = penalizaciones
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

            await GuardarReporteAsync(TipoReporte.UsuariosMasPenalizados, JsonSerializer.Serialize(new { Top = top }), JsonSerializer.Serialize(resultado));

            return resultado;
        }

        // Devuelve el listado consolidado de todas las sanciones vigentes en el sistema.
        public async Task<IEnumerable<PenalizacionDTO>> ObtenerPenalizacionesActivasAsync()
        {
            var penalizaciones = await _penalizacionRepository.GetActivasAsync();
            var resultado = penalizaciones.Select(PenalizacionMapper.ToDTO).ToList();

            await GuardarReporteAsync(TipoReporte.PenalizacionesActivas, "{}", JsonSerializer.Serialize(resultado));

            return resultado;
        }

        public async Task<IEnumerable<HistorialReporteDTO>> ObtenerHistorialReportesAsync(int count = 10)
        {
            var reportes = await _reporteRepository.GetUltimosReportesAsync(count);
            return reportes.Select(ReporteMapper.ToDTO);
        }

        private async Task GuardarReporteAsync(TipoReporte tipo, string parametros, string resultado)
        {
            try
            {
                var reporte = new Reporte(
                    _guidGenerator.Create(),
                    tipo,
                    parametros,
                    resultado,
                    null // TODO: Implementar obtención de usuario actual si es necesario
                );

                await _reporteRepository.AddAsync(reporte);
                await _unitOfWork.SaveChangesAsync();
            }
            catch
            {
                // Silencioso para no romper la generación del reporte si falla el guardado
            }
        }
    }
}
