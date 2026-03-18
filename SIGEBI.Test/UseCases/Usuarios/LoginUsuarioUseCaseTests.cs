using FluentAssertions;
using Moq;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Business.Interfaces.Services;
using SIGEBI.Business.UseCases.Usuarios;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Enums.Seguridad;
using Xunit;

namespace SIGEBI.Test.UseCases.Usuarios
{
    public class LoginUsuarioUseCaseTests
    {
        private readonly Mock<IUsuarioRepository> _usuarioRepo;
        private readonly Mock<IHashService> _hashService;
        private readonly LoginUsuarioUseCase _useCase;

        public LoginUsuarioUseCaseTests()
        {
            _usuarioRepo = new Mock<IUsuarioRepository>();
            _hashService = new Mock<IHashService>();

            _useCase = new LoginUsuarioUseCase(
                _usuarioRepo.Object,
                _hashService.Object);
        }

        // ── HELPER ──

        private Usuario CrearUsuario()
            => new Usuario("Juan Perez", "juan@test.com", "hash123", RolUsuario.Estudiante);

        // ── PRUEBAS ──

        [Fact]
        public async Task Ejecutar_CredencialesValidas_DevuelveUsuario()
        {
            // Arrange
            var usuario = CrearUsuario();

            _usuarioRepo.Setup(r => r.GetByCorreoAsync("juan@test.com")).ReturnsAsync(usuario);
            _hashService.Setup(h => h.Verificar("password123", "hash123")).Returns(true);

            // Act
            var resultado = await _useCase.EjecutarAsync("juan@test.com", "password123");

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Correo.Should().Be("juan@test.com");
        }

        [Fact]
        public async Task Ejecutar_CorreoNoRegistrado_DevuelveNull()
        {
            // Arrange
            _usuarioRepo.Setup(r => r.GetByCorreoAsync("noexiste@test.com")).ReturnsAsync((Usuario?)null);

            // Act
            var resultado = await _useCase.EjecutarAsync("noexiste@test.com", "password123");

            // Assert
            resultado.Should().BeNull();
        }

        [Fact]
        public async Task Ejecutar_PasswordIncorrecto_DevuelveNull()
        {
            // Arrange
            var usuario = CrearUsuario();

            _usuarioRepo.Setup(r => r.GetByCorreoAsync("juan@test.com")).ReturnsAsync(usuario);
            _hashService.Setup(h => h.Verificar("passwordIncorrecto", "hash123")).Returns(false);

            // Act
            var resultado = await _useCase.EjecutarAsync("juan@test.com", "passwordIncorrecto");

            // Assert
            resultado.Should().BeNull();
        }

        [Fact]
        public async Task Ejecutar_CredencialesValidas_VerificaHashCorrectamente()
        {
            // Arrange
            var usuario = CrearUsuario();

            _usuarioRepo.Setup(r => r.GetByCorreoAsync("juan@test.com")).ReturnsAsync(usuario);
            _hashService.Setup(h => h.Verificar("password123", "hash123")).Returns(true);

            // Act
            await _useCase.EjecutarAsync("juan@test.com", "password123");

            // Assert 
            _hashService.Verify(h => h.Verificar("password123", "hash123"), Times.Once);
        }
    }
}
