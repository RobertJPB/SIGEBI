using FluentAssertions;
using Moq;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.UseCases.Prestamos;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Enums.Seguridad;
using Xunit;

namespace SIGEBI.Test.UseCases.Prestamos
{
    public class ConsultarPrestamoUseCaseTests
    {
        private readonly Mock<IPrestamoRepository> _prestamoRepo;
        private readonly Mock<IUsuarioRepository> _usuarioRepo;
        private readonly ConsultarPrestamoUseCase _useCase;

        public ConsultarPrestamoUseCaseTests()
        {
            _prestamoRepo = new Mock<IPrestamoRepository>();
            _usuarioRepo = new Mock<IUsuarioRepository>();

            _useCase = new ConsultarPrestamoUseCase(
                _prestamoRepo.Object,
                _usuarioRepo.Object);
        }

        // ── HELPERS ──

        private Usuario CrearUsuario()
            => new Usuario("Juan Perez", "juan@test.com", "hash123", RolUsuario.Estudiante);

        private Prestamo CrearPrestamo(Guid usuarioId)
            => new Prestamo(usuarioId, Guid.NewGuid(), 7, DateTime.UtcNow);

        // ── OBTENER TODOS ──

        [Fact]
        public async Task ObtenerTodos_HayPrestamos_DevuelveLista()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var prestamos = new List<Prestamo>
            {
                CrearPrestamo(usuarioId),
                CrearPrestamo(usuarioId)
            };

            _prestamoRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(prestamos);

            // Act
            var resultado = await _useCase.ObtenerTodosAsync();

            // Assert
            resultado.Should().HaveCount(2);
        }

        [Fact]
        public async Task ObtenerTodos_NoPrestamos_DevuelveListaVacia()
        {
            // Arrange
            _prestamoRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Prestamo>());

            // Act
            var resultado = await _useCase.ObtenerTodosAsync();

            // Assert
            resultado.Should().BeEmpty();
        }

        // ── OBTENER POR USUARIO ──

        [Fact]
        public async Task ObtenerPorUsuario_UsuarioExistente_DevuelvePrestamos()
        {
            // Arrange
            var usuario = CrearUsuario();
            var prestamos = new List<Prestamo> { CrearPrestamo(usuario.Id) };

            _usuarioRepo.Setup(r => r.GetByIdAsync(usuario.Id)).ReturnsAsync(usuario);
            _prestamoRepo.Setup(r => r.GetByUsuarioIdAsync(usuario.Id)).ReturnsAsync(prestamos);

            // Act
            var resultado = await _useCase.ObtenerPorUsuarioAsync(usuario.Id);

            // Assert
            resultado.Should().HaveCount(1);
        }

        [Fact]
        public async Task ObtenerPorUsuario_UsuarioNoEncontrado_LanzaExcepcion()
        {
            // Arrange
            _usuarioRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Usuario?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _useCase.ObtenerPorUsuarioAsync(Guid.NewGuid()));
        }

        // ── OBTENER ACTIVOS POR USUARIO ──

        [Fact]
        public async Task ObtenerActivosPorUsuario_DevuelveSoloActivos()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var prestamos = new List<Prestamo> { CrearPrestamo(usuarioId) };

            _prestamoRepo.Setup(r => r.GetActivosByUsuarioIdAsync(usuarioId)).ReturnsAsync(prestamos);

            // Act
            var resultado = await _useCase.ObtenerActivosPorUsuarioAsync(usuarioId);

            // Assert
            resultado.Should().HaveCount(1);
        }

        // ── OBTENER ATRASADOS ──

        [Fact]
        public async Task ObtenerAtrasados_HayAtrasados_DevuelveLista()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var prestamos = new List<Prestamo> { CrearPrestamo(usuarioId) };

            _prestamoRepo.Setup(r => r.GetAtrasadosAsync()).ReturnsAsync(prestamos);

            // Act
            var resultado = await _useCase.ObtenerAtrasadosAsync();

            // Assert
            resultado.Should().HaveCount(1);
        }
    }
}
