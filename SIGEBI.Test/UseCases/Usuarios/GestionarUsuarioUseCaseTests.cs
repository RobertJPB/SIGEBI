using FluentAssertions;
using Moq;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.UseCases.Usuarios;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Enums.Seguridad;
using Xunit;

namespace SIGEBI.Test.UseCases.Usuarios
{
    public class GestionarUsuarioUseCaseTests
    {
        private readonly Mock<IUsuarioRepository> _usuarioRepo;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly GestionarUsuarioUseCase _useCase;

        public GestionarUsuarioUseCaseTests()
        {
            _usuarioRepo = new Mock<IUsuarioRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();

            _useCase = new GestionarUsuarioUseCase(
                _usuarioRepo.Object,
                _unitOfWork.Object);
        }

        // â”€â”€ HELPER â”€â”€

        private Usuario CrearUsuario()
            => new Usuario(Guid.NewGuid(), "Juan Perez", "juan@test.com", "hash123", RolUsuario.Estudiante);

        // â”€â”€ OBTENER TODOS â”€â”€

        [Fact]
        public async Task ObtenerTodos_HayUsuarios_DevuelveLista()
        {
            // Arrange
            var usuarios = new List<Usuario> { CrearUsuario(), CrearUsuario() };
            _usuarioRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(usuarios);

            // Act
            var resultado = await _useCase.ObtenerTodosAsync();

            // Assert
            resultado.Should().HaveCount(2);
        }

        // â”€â”€ OBTENER POR ID â”€â”€

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

        // â”€â”€ ACTIVAR â”€â”€

        [Fact]
        public async Task Activar_UsuarioExistente_CambiaEstadoActivo()
        {
            // Arrange
            var usuario = CrearUsuario();
            usuario.Desactivar();

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

        // â”€â”€ DESACTIVAR â”€â”€

        [Fact]
        public async Task Desactivar_UsuarioExistente_CambiaEstadoInactivo()
        {
            // Arrange
            var usuario = CrearUsuario();

            _usuarioRepo.Setup(r => r.GetByIdAsync(usuario.Id)).ReturnsAsync(usuario);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _useCase.DesactivarAsync(usuario.Id);

            // Assert
            usuario.Estado.Should().Be(EstadoUsuario.Inactivo);
        }

        // â”€â”€ BLOQUEAR â”€â”€

        [Fact]
        public async Task Bloquear_UsuarioExistente_CambiaEstadoBloqueado()
        {
            // Arrange
            var usuario = CrearUsuario();

            _usuarioRepo.Setup(r => r.GetByIdAsync(usuario.Id)).ReturnsAsync(usuario);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _useCase.BloquearAsync(usuario.Id);

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
