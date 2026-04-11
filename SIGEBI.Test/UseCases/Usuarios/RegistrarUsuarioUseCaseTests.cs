using FluentAssertions;
using Moq;
using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.Interfaces.Services;
using SIGEBI.Business.UseCases.Usuarios;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Enums.Seguridad;
using SIGEBI.Business.Interfaces.Common;
using SIGEBI.Domain.ValueObjects;
using Xunit;

namespace SIGEBI.Test.UseCases.Usuarios
{
    public class RegistrarUsuarioUseCaseTests
    {
        private readonly Mock<IUsuarioRepository> _usuarioRepo;
        private readonly Mock<IHashService> _hashService;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IEmailAdapter> _emailAdapter;
        private readonly RegistrarUsuarioUseCase _useCase;

        public RegistrarUsuarioUseCaseTests()
        {
            _usuarioRepo = new Mock<IUsuarioRepository>();
            _hashService = new Mock<IHashService>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _emailAdapter = new Mock<IEmailAdapter>();

            _useCase = new RegistrarUsuarioUseCase(
                _usuarioRepo.Object,
                _hashService.Object,
                _unitOfWork.Object,
                new Mock<IGuidGenerator>().Object,
                _emailAdapter.Object);
        }

        // ── HELPER ──

        private UsuarioDTO CrearDTO(string correo = "juan@test.com")
            => new UsuarioDTO
            {
                Nombre = "Juan Perez",
                Correo = correo,
                Contrasena = "password123",
                IdRol = (int)RolUsuario.Estudiante
            };

        // ── PRUEBAS ──

        [Fact]
        public async Task Ejecutar_UsuarioNuevo_RegistraCorrectamente()
        {
            // Arrange
            var dto = CrearDTO();

            _usuarioRepo.Setup(r => r.GetByCorreoAsync(dto.Correo)).ReturnsAsync((Usuario?)null);
            _hashService.Setup(h => h.Hash(dto.Contrasena)).Returns("hash123");
            _usuarioRepo.Setup(r => r.AddAsync(It.IsAny<Usuario>())).Returns(Task.CompletedTask);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _useCase.EjecutarAsync(dto);

            // Assert
            _usuarioRepo.Verify(r => r.AddAsync(It.IsAny<Usuario>()), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_CorreoYaRegistrado_LanzaExcepcion()
        {
            // Arrange
            var dto = CrearDTO();
            var existente = new Usuario(Guid.NewGuid(), "Juan Perez", new Email("juan@test.com"), "hash123", RolUsuario.Estudiante);

            _usuarioRepo.Setup(r => r.GetByCorreoAsync(dto.Correo)).ReturnsAsync(existente);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _useCase.EjecutarAsync(dto));
        }

        [Fact]
        public async Task Ejecutar_UsuarioNuevo_HashearPasswordAntesDeGuardar()
        {
            // Arrange
            var dto = CrearDTO();

            _usuarioRepo.Setup(r => r.GetByCorreoAsync(dto.Correo)).ReturnsAsync((Usuario?)null);
            _hashService.Setup(h => h.Hash(dto.Contrasena)).Returns("hash123");
            _usuarioRepo.Setup(r => r.AddAsync(It.IsAny<Usuario>())).Returns(Task.CompletedTask);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _useCase.EjecutarAsync(dto);

            // Assert 
            _hashService.Verify(h => h.Hash(dto.Contrasena), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_CorreoYaRegistrado_NoGuardaUsuario()
        {
            // Arrange
            var dto = CrearDTO();
            var existente = new Usuario(Guid.NewGuid(), "Juan Perez", new Email("juan@test.com"), "hash123", RolUsuario.Estudiante);

            _usuarioRepo.Setup(r => r.GetByCorreoAsync(dto.Correo)).ReturnsAsync(existente);

            // Act
            try { await _useCase.EjecutarAsync(dto); } catch { }

            
            _usuarioRepo.Verify(r => r.AddAsync(It.IsAny<Usuario>()), Times.Never);
        }
    }
}
