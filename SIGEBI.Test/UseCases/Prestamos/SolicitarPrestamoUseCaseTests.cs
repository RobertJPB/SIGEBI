using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.Interfaces.Services;
using SIGEBI.Business.UseCases.Prestamos;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Entities.Recursos;
using SIGEBI.Domain.Enums.Seguridad;
using SIGEBI.Domain.DomainServices;
using SIGEBI.Business.Interfaces.Common;
using Xunit;

namespace SIGEBI.Test.UseCases.Prestamos
{
    public class SolicitarPrestamoUseCaseTests
    {
        private readonly Mock<IPrestamoRepository> _prestamoRepo;
        private readonly Mock<IUsuarioRepository> _usuarioRepo;
        private readonly Mock<IRecursoRepository> _recursoRepo;
        private readonly Mock<IPenalizacionRepository> _penalizacionRepo;
        private readonly Mock<INotificacionRepository> _notificacionRepo;
        private readonly Mock<ISolicitudAccesoRepository> _solicitudAccesoRepo;
        private readonly Mock<IEmailAdapter> _emailAdapter;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly SolicitarPrestamoUseCase _useCase;

        public SolicitarPrestamoUseCaseTests()
        {
            _prestamoRepo = new Mock<IPrestamoRepository>();
            _usuarioRepo = new Mock<IUsuarioRepository>();
            _recursoRepo = new Mock<IRecursoRepository>();
            _penalizacionRepo = new Mock<IPenalizacionRepository>();
            _notificacionRepo = new Mock<INotificacionRepository>();
            _solicitudAccesoRepo = new Mock<ISolicitudAccesoRepository>();
            _emailAdapter = new Mock<IEmailAdapter>();
            _unitOfWork = new Mock<IUnitOfWork>();

            // Setup UnitOfWork
            _unitOfWork.Setup(u => u.SolicitudesAcceso).Returns(_solicitudAccesoRepo.Object);

            _useCase = new SolicitarPrestamoUseCase(
                _prestamoRepo.Object,
                _usuarioRepo.Object,
                _recursoRepo.Object,
                _penalizacionRepo.Object,
                _notificacionRepo.Object,
                _emailAdapter.Object,
                _unitOfWork.Object,
                new Mock<IMemoryCache>().Object,
                new Mock<IGuidGenerator>().Object,
                Microsoft.Extensions.Logging.Abstractions.NullLogger<SolicitarPrestamoUseCase>.Instance,
                new Mock<IAuditService>().Object);
        }

        // â”€â”€ HELPERS â”€â”€

        private Usuario CrearUsuarioActivo(RolUsuario rol = RolUsuario.Estudiante)
            => new Usuario(Guid.NewGuid(), "Juan Perez", "juan@test.com", "hash123", rol);

        private Libro CrearLibroDisponible()
            => new Libro(Guid.NewGuid(), "El Principito", "Antoine", 1, 5, null, "978-84-261", "Editorial X", 1943);

        // â”€â”€ PRUEBAS â”€â”€

        [Fact]
        public async Task Ejecutar_UsuarioYRecursoValidos_DevuelveDTO()
        {
            // Arrange
            var usuario = CrearUsuarioActivo();
            var libro = CrearLibroDisponible();

            _usuarioRepo.Setup(r => r.GetByIdAsync(usuario.Id)).ReturnsAsync(usuario);
            _recursoRepo.Setup(r => r.GetByIdAsync(libro.Id)).ReturnsAsync(libro);
            _prestamoRepo.Setup(r => r.GetByUsuarioIdAsync(usuario.Id)).ReturnsAsync(new List<Prestamo>());
            _penalizacionRepo.Setup(r => r.GetByUsuarioIdAsync(usuario.Id)).ReturnsAsync(new List<Penalizacion>());
            _prestamoRepo.Setup(r => r.AddAsync(It.IsAny<Prestamo>())).Returns(Task.CompletedTask);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var resultado = await _useCase.EjecutarAsync(usuario.Id, libro.Id);

            // Assert
            resultado.Should().NotBeNull();
            resultado.UsuarioId.Should().Be(usuario.Id);
            resultado.RecursoId.Should().Be(libro.Id);
        }

        [Fact]
        public async Task Ejecutar_UsuarioNoEncontrado_LanzaExcepcion()
        {
            // Arrange
            _usuarioRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Usuario?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _useCase.EjecutarAsync(Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public async Task Ejecutar_RecursoNoEncontrado_LanzaExcepcion()
        {
            // Arrange
            var usuario = CrearUsuarioActivo();
            _usuarioRepo.Setup(r => r.GetByIdAsync(usuario.Id)).ReturnsAsync(usuario);
            _recursoRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((RecursoBibliografico?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _useCase.EjecutarAsync(usuario.Id, Guid.NewGuid()));
        }

        [Fact]
        public async Task Ejecutar_UsuarioConPenalizacionActiva_LanzaExcepcion()
        {
            // Arrange
            var usuario = CrearUsuarioActivo();
            var libro = CrearLibroDisponible();
            var penalizacion = new Penalizacion(Guid.NewGuid(), usuario.Id, "Atraso", 5, DateTime.UtcNow, null);

            _usuarioRepo.Setup(r => r.GetByIdAsync(usuario.Id)).ReturnsAsync(usuario);
            _recursoRepo.Setup(r => r.GetByIdAsync(libro.Id)).ReturnsAsync(libro);
            _prestamoRepo.Setup(r => r.GetByUsuarioIdAsync(usuario.Id)).ReturnsAsync(new List<Prestamo>());
            _penalizacionRepo.Setup(r => r.GetByUsuarioIdAsync(usuario.Id)).ReturnsAsync(new List<Penalizacion> { penalizacion });

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _useCase.EjecutarAsync(usuario.Id, libro.Id));
        }

        [Fact]
        public async Task Ejecutar_RecursoSinStock_LanzaExcepcion()
        {
            // Arrange
            var usuario = CrearUsuarioActivo();
            var libro = new Libro(Guid.NewGuid(), "El Principito", "Antoine", 1, 0, null, "978-84-261", "Editorial X", 1943);

            _usuarioRepo.Setup(r => r.GetByIdAsync(usuario.Id)).ReturnsAsync(usuario);
            _recursoRepo.Setup(r => r.GetByIdAsync(libro.Id)).ReturnsAsync(libro);
            _prestamoRepo.Setup(r => r.GetByUsuarioIdAsync(usuario.Id)).ReturnsAsync(new List<Prestamo>());
            _penalizacionRepo.Setup(r => r.GetByUsuarioIdAsync(usuario.Id)).ReturnsAsync(new List<Penalizacion>());

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _useCase.EjecutarAsync(usuario.Id, libro.Id));
        }

        [Fact]
        public async Task Ejecutar_Exitoso_DisminuyeStockDelRecurso()
        {
            // Arrange
            var usuario = CrearUsuarioActivo();
            var libro = CrearLibroDisponible();
            var stockInicial = libro.Stock;

            _usuarioRepo.Setup(r => r.GetByIdAsync(usuario.Id)).ReturnsAsync(usuario);
            _recursoRepo.Setup(r => r.GetByIdAsync(libro.Id)).ReturnsAsync(libro);
            _prestamoRepo.Setup(r => r.GetByUsuarioIdAsync(usuario.Id)).ReturnsAsync(new List<Prestamo>());
            _penalizacionRepo.Setup(r => r.GetByUsuarioIdAsync(usuario.Id)).ReturnsAsync(new List<Penalizacion>());
            _prestamoRepo.Setup(r => r.AddAsync(It.IsAny<Prestamo>())).Returns(Task.CompletedTask);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _useCase.EjecutarAsync(usuario.Id, libro.Id);

            // Assert
            libro.Stock.Should().Be(stockInicial - 1);
        }
        [Fact]
        public async Task Ejecutar_ConFechaPersonalizada_RespetaLaFecha()
        {
            // Arrange
            var usuario = CrearUsuarioActivo();
            var libro = CrearLibroDisponible();
            var fechaPersonalizada = DateTime.UtcNow.AddDays(10);

            _usuarioRepo.Setup(r => r.GetByIdAsync(usuario.Id)).ReturnsAsync(usuario);
            _recursoRepo.Setup(r => r.GetByIdAsync(libro.Id)).ReturnsAsync(libro);
            _prestamoRepo.Setup(r => r.GetByUsuarioIdAsync(usuario.Id)).ReturnsAsync(new List<Prestamo>());
            _penalizacionRepo.Setup(r => r.GetByUsuarioIdAsync(usuario.Id)).ReturnsAsync(new List<Penalizacion>());
            _prestamoRepo.Setup(r => r.AddAsync(It.IsAny<Prestamo>())).Returns(Task.CompletedTask);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var resultado = await _useCase.EjecutarAsync(usuario.Id, libro.Id, fechaPersonalizada);

            // Assert
            resultado.FechaDevolucionEstimada.Should().Be(fechaPersonalizada.ToString("yyyy-MM-dd"));
        }
        [Fact]
        public async Task Ejecutar_FechaExcedeLimite_LanzaExcepcion()
        {
            // Arrange
            var usuario = CrearUsuarioActivo();
            var libro = CrearLibroDisponible();
            var fechaExcedida = DateTime.UtcNow.AddDays(PrestamoPolicy.MaxDiasPrestamoTotal + 1);

            _usuarioRepo.Setup(r => r.GetByIdAsync(usuario.Id)).ReturnsAsync(usuario);
            _recursoRepo.Setup(r => r.GetByIdAsync(libro.Id)).ReturnsAsync(libro);
            _prestamoRepo.Setup(r => r.GetActivosByUsuarioIdAsync(usuario.Id)).ReturnsAsync(new List<Prestamo>());
            _penalizacionRepo.Setup(r => r.GetByUsuarioIdAsync(usuario.Id)).ReturnsAsync(new List<Penalizacion>());

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _useCase.EjecutarAsync(usuario.Id, libro.Id, fechaExcedida));
        }

        [Fact]
        public async Task Ejecutar_UsuarioConPrestamoVencido_LanzaExcepcion()
        {
            // Arrange
            var usuario = CrearUsuarioActivo();
            var libro = CrearLibroDisponible();
            
            // Simulamos un préstamo vencido
            var prestamoVencido = new Prestamo(Guid.NewGuid(), usuario.Id, Guid.NewGuid(), 15, DateTime.UtcNow.AddDays(-20));
            prestamoVencido.MarcarAtrasadoSiAplica(DateTime.UtcNow);

            _usuarioRepo.Setup(r => r.GetByIdAsync(usuario.Id)).ReturnsAsync(usuario);
            _recursoRepo.Setup(r => r.GetByIdAsync(libro.Id)).ReturnsAsync(libro);
            _prestamoRepo.Setup(r => r.GetByUsuarioIdAsync(usuario.Id)).ReturnsAsync(new List<Prestamo> { prestamoVencido });
            _penalizacionRepo.Setup(r => r.GetByUsuarioIdAsync(usuario.Id)).ReturnsAsync(new List<Penalizacion>());

            // Act & Assert
            var action = () => _useCase.EjecutarAsync(usuario.Id, libro.Id);
            await action.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*vencida*");
                
            // Verificar que se registró el rechazo en la trazabilidad
            _solicitudAccesoRepo.Verify(r => r.AddAsync(It.Is<SolicitudAcceso>(s => !s.FueAprobada)), Times.Once);
        }
    }
}
