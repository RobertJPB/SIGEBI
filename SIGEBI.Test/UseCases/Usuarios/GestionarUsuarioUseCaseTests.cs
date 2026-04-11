using FluentAssertions;
using Moq;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.UseCases.Usuarios;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.UseCases.Usuarios;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Enums.Seguridad;
using SIGEBI.Domain.ValueObjects;
using Xunit;

namespace SIGEBI.Test.UseCases.Usuarios
{
    public class GestionarUsuarioUseCaseTests
    {
        private readonly Mock<IUsuarioRepository> _usuarioRepo;
        private readonly Mock<INotificacionesUseCase> _notificaciones;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<Microsoft.Extensions.Caching.Memory.IMemoryCache> _cache;
        private readonly GestionarUsuarioUseCase _useCase;

        public GestionarUsuarioUseCaseTests()
        {
            _usuarioRepo = new Mock<IUsuarioRepository>();
            _notificaciones = new Mock<INotificacionesUseCase>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _cache = new Mock<Microsoft.Extensions.Caching.Memory.IMemoryCache>();

            _useCase = new GestionarUsuarioUseCase(
                _usuarioRepo.Object,
                _notificaciones.Object,
                _unitOfWork.Object,
                _cache.Object);
        }

        // ── HELPER ──

        private Usuario CrearUsuario()
            => new Usuario(Guid.NewGuid(), "Juan Perez", new Email("juan@test.com"), "hash123", RolUsuario.Estudiante);

        // ── OBTENER TODOS ──

        [Fact]
        public async Task ObtenerTodos_HayUsuarios_DevuelveLista()
        {
            // Arrange
            var usuarios = new List<Usuario> { CrearUsuario(), CrearUsuario() };
            _usuarioRepo.Setup(r => r.GetAllConPenalizacionesAsync()).ReturnsAsync(usuarios);

            // Act
            var resultado = await _useCase.ObtenerTodosAsync();

            // Assert
            resultado.Should().HaveCount(2);
        }

        // ── OBTENER POR ID ──

        [Fact]
        public async Task ObtenerPorId_UsuarioExistente_DevuelveDTO()
        {
            // Arrange
            var usuario = CrearUsuario();
            _usuarioRepo.Setup(r => r.GetByIdAsync(usuario.Id)).ReturnsAsync(usuario);

            // Act
            var resultado = await _useCase.ObtenerPorIdAsync(usuario.Id);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Correo.Should().Be("juan@test.com");
        }

        [Fact]
        public async Task ObtenerPorId_UsuarioNoExistente_DevuelveNull()
        {
            // Arrange
            _usuarioRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Usuario?)null);

            // Act
            var resultado = await _useCase.ObtenerPorIdAsync(Guid.NewGuid());

            // Assert
            resultado.Should().BeNull();
        }

        // ── ACTIVAR ──

        [Fact]
        public async Task Activar_UsuarioExistente_CambiaEstadoActivo()
        {
            // Arrange
            var usuario = CrearUsuario();
            usuario.Desactivar("Motivo de prueba");

            _usuarioRepo.Setup(r => r.GetByIdAsync(usuario.Id)).ReturnsAsync(usuario);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _useCase.ActivarAsync(usuario.Id);

            // Assert
            usuario.Estado.Should().Be(EstadoUsuario.Activo);
        }

        [Fact]
        public async Task Activar_UsuarioNoEncontrado_LanzaExcepcion()
        {
            // Arrange
            _usuarioRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Usuario?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _useCase.ActivarAsync(Guid.NewGuid()));
        }

        // ── DESACTIVAR ──

        [Fact]
        public async Task Desactivar_UsuarioExistente_CambiaEstadoInactivo()
        {
            // Arrange
            var usuario = CrearUsuario();

            _usuarioRepo.Setup(r => r.GetByIdAsync(usuario.Id)).ReturnsAsync(usuario);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _useCase.DesactivarAsync(usuario.Id, "Motivo de prueba");

            // Assert
            usuario.Estado.Should().Be(EstadoUsuario.Inactivo);
        }

        // ── BLOQUEAR ──

        [Fact]
        public async Task Bloquear_UsuarioExistente_CambiaEstadoBloqueado()
        {
            // Arrange
            var usuario = CrearUsuario();

            _usuarioRepo.Setup(r => r.GetByIdAsync(usuario.Id)).ReturnsAsync(usuario);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _useCase.BloquearAsync(usuario.Id, "Motivo de prueba");

            // Assert
            usuario.Estado.Should().Be(EstadoUsuario.Bloqueado);
        }

        // CAMBIAR ROL 

        [Fact]
        public async Task CambiarRol_UsuarioExistente_ActualizaRol()
        {
            // Arrange
            var usuario = CrearUsuario();

            _usuarioRepo.Setup(r => r.GetByIdAsync(usuario.Id)).ReturnsAsync(usuario);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _useCase.CambiarRolAsync(usuario.Id, RolUsuario.Bibliotecario);

            // Assert
            usuario.Rol.Should().Be(RolUsuario.Bibliotecario);
        }

        [Fact]
        public async Task CambiarRol_UsuarioNoEncontrado_LanzaExcepcion()
        {
            // Arrange
            _usuarioRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Usuario?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _useCase.CambiarRolAsync(Guid.NewGuid(), RolUsuario.Bibliotecario));
        }
    }
}
