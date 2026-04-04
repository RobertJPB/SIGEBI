using FluentAssertions;
using Moq;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.UseCases.Usuarios;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Entities.Recursos;
using SIGEBI.Domain.Enums.Seguridad;
using Xunit;

namespace SIGEBI.Test.UseCases.Usuarios
{
    public class GenerarReportesUseCaseTests
    {
        private readonly Mock<IPrestamoRepository> _prestamoRepo;
        private readonly Mock<IUsuarioRepository> _usuarioRepo;
        private readonly Mock<IRecursoRepository> _recursoRepo;
        private readonly Mock<IPenalizacionRepository> _penalizacionRepo;
        private readonly GenerarReportesUseCase _useCase;

        public GenerarReportesUseCaseTests()
        {
            _prestamoRepo = new Mock<IPrestamoRepository>();
            _usuarioRepo = new Mock<IUsuarioRepository>();
            _recursoRepo = new Mock<IRecursoRepository>();
            _penalizacionRepo = new Mock<IPenalizacionRepository>();

            _useCase = new GenerarReportesUseCase(
                _prestamoRepo.Object,
                _usuarioRepo.Object,
                _recursoRepo.Object,
                _penalizacionRepo.Object);
        }

        // â”€â”€ GENERAR REPORTE GENERAL â”€â”€

        // Caso de Uso: Generar Reportes - Proceso: Generar un resumen ejecutivo con mÃ©tricas de todas las entidades.
        [Fact]
        public async Task GenerarReporteGeneral_CalculaTotalesCorrectamente()
        {
            // Arrange
            _prestamoRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Prestamo> { new Prestamo(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 7, DateTime.UtcNow) });
            _usuarioRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Usuario> { new Usuario(Guid.NewGuid(), "Test", "t@t.com", "h", RolUsuario.Estudiante) });
            _recursoRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<RecursoBibliografico> { new Libro(Guid.NewGuid(), "T", "A", 1, 1, null, "I", "E", 2020) });
            _penalizacionRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Penalizacion> { new Penalizacion(Guid.NewGuid(), Guid.NewGuid(), "M", 3, DateTime.UtcNow, Guid.NewGuid()) });

            // Act
            var resultado = await _useCase.GenerarReporteGeneralAsync();

            // Assert
            resultado.TotalPrestamos.Should().Be(1);
            resultado.TotalUsuarios.Should().Be(1);
            resultado.TotalRecursos.Should().Be(1);
            resultado.TotalPenalizaciones.Should().Be(1);
        }

        // â”€â”€ OBTENER PRESTAMOS POR PERIODO â”€â”€

        // Caso de Uso: Generar Reportes - Proceso: Filtrar prÃ©stamos realizados dentro de un intervalo de fechas.
        [Fact]
        public async Task ObtenerPrestamosPorPeriodo_FiltraPorFechas()
        {
            // Arrange
            var fechaInicio = DateTime.UtcNow.AddDays(-10);
            var fechaFin = DateTime.UtcNow;
            var prestamos = new List<Prestamo>
            {
                new Prestamo(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 7, DateTime.UtcNow.AddDays(-5)), // Dentro
                new Prestamo(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 7, DateTime.UtcNow.AddDays(-15)) // Fuera
            };
            _prestamoRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(prestamos);

            // Act
            var resultado = await _useCase.ObtenerPrestamosPorPeriodoAsync(fechaInicio, fechaFin);

            // Assert
            resultado.Should().HaveCount(1);
        }

        // â”€â”€ USUARIOS MAS PENALIZADOS â”€â”€

        // Caso de Uso: Generar Reportes - Proceso: Identificar a los usuarios con mayor cantidad de sanciones.
        [Fact]
        public async Task ObtenerUsuariosMasPenalizados_AgrupaYOrdena()
        {
            // Arrange
            var user1 = Guid.NewGuid();
            var user2 = Guid.NewGuid();
            var penalizaciones = new List<Penalizacion>
            {
                new Penalizacion(Guid.NewGuid(), user1, "M", 1, DateTime.UtcNow, Guid.NewGuid()),
                new Penalizacion(Guid.NewGuid(), user1, "M", 2, DateTime.UtcNow, Guid.NewGuid()),
                new Penalizacion(Guid.NewGuid(), user2, "M", 1, DateTime.UtcNow, Guid.NewGuid())
            };
            _penalizacionRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(penalizaciones);

            // Act
            var resultado = await _useCase.ObtenerUsuariosMasPenalizadosAsync(5);

            // Assert
            resultado.Should().HaveCount(2);
            // El primero debe ser user1 con 2 sanciones (dinÃ¡mico porque devuelve object)
            var lista = resultado.ToList();
            lista[0].GetType().GetProperty("UsuarioId")?.GetValue(lista[0], null).Should().Be(user1);
        }

        // â”€â”€ PENALIZACIONES ACTIVAS â”€â”€

        // Caso de Uso: Generar Reportes - Proceso: Obtener listado de sanciones vigentes en el sistema.
        [Fact]
        public async Task ObtenerPenalizacionesActivas_LlamaAlRepositorio()
        {
            // Arrange
            _penalizacionRepo.Setup(r => r.GetActivasAsync()).ReturnsAsync(new List<Penalizacion>());

            // Act
            await _useCase.ObtenerPenalizacionesActivasAsync();

            // Assert
            _penalizacionRepo.Verify(r => r.GetActivasAsync(), Times.Once);
        }
    }
}
