using FluentAssertions;
using Moq;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.UseCases.Prestamos;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Entities.Recursos;
using SIGEBI.Domain.Enums.Biblioteca;
using SIGEBI.Domain.Enums.Seguridad;
using SIGEBI.Business.Interfaces.Common;
using Xunit;

namespace SIGEBI.Test.UseCases.Prestamos
{
    public class DevolverPrestamoUseCaseTests
    {
        private readonly Mock<IPrestamoRepository> _prestamoRepo;
        private readonly Mock<IRecursoRepository> _recursoRepo;
        private readonly Mock<IPenalizacionRepository> _penalizacionRepo;
        private readonly Mock<INotificacionRepository> _notificacionRepo;
        private readonly Mock<IListaDeseosRepository> _listaDeseosRepo;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly DevolverPrestamoUseCase _useCase;

        public DevolverPrestamoUseCaseTests()
        {
            _prestamoRepo = new Mock<IPrestamoRepository>();
            _recursoRepo = new Mock<IRecursoRepository>();
            _penalizacionRepo = new Mock<IPenalizacionRepository>();
            _notificacionRepo = new Mock<INotificacionRepository>();
            _listaDeseosRepo = new Mock<IListaDeseosRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();

            _useCase = new DevolverPrestamoUseCase(
                _prestamoRepo.Object,
                _recursoRepo.Object,
                _penalizacionRepo.Object,
                _notificacionRepo.Object,
                _listaDeseosRepo.Object,
                _unitOfWork.Object,
                new Mock<IGuidGenerator>().Object);
        }

        // â”€â”€ HELPERS â”€â”€

        private Libro CrearLibroDisponible()
            => new Libro(Guid.NewGuid(), "El Principito", "Antoine", 1, 5, null, "978-84-261", "Editorial X", 1943);

        private Prestamo CrearPrestamoActivo(Guid? usuarioId = null, Guid? recursoId = null)
            => new Prestamo(Guid.NewGuid(), 
                usuarioId ?? Guid.NewGuid(),
                recursoId ?? Guid.NewGuid(),
                7,
                DateTime.UtcNow.AddDays(-3));

        // â”€â”€ PRUEBAS â”€â”€

        [Fact]
        public async Task Ejecutar_PrestamoExistente_DevuelveCorrectamente()
        {
            // Arrange
            var libro = CrearLibroDisponible();
            var prestamo = CrearPrestamoActivo(recursoId: libro.Id);

            _prestamoRepo.Setup(r => r.GetByIdAsync(prestamo.Id)).ReturnsAsync(prestamo);
            _recursoRepo.Setup(r => r.GetByIdAsync(prestamo.RecursoId)).ReturnsAsync(libro);
            _penalizacionRepo.Setup(r => r.AddAsync(It.IsAny<Penalizacion>())).Returns(Task.CompletedTask);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _useCase.EjecutarAsync(prestamo.Id);

            // Assert
            prestamo.FechaDevolucionReal.Should().NotBeNull();
            prestamo.EstadoActual.Should().Be(EstadoPrestamo.Devuelto);
        }

        [Fact]
        public async Task Ejecutar_PrestamoNoEncontrado_LanzaExcepcion()
        {
            // Arrange
            _prestamoRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Prestamo?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _useCase.EjecutarAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task Ejecutar_DevolucionExitosa_AumentaStockDelRecurso()
        {
            // Arrange
            var libro = CrearLibroDisponible();
            var prestamo = CrearPrestamoActivo(recursoId: libro.Id);
            var stockInicial = libro.Stock;

            _prestamoRepo.Setup(r => r.GetByIdAsync(prestamo.Id)).ReturnsAsync(prestamo);
            _recursoRepo.Setup(r => r.GetByIdAsync(prestamo.RecursoId)).ReturnsAsync(libro);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _useCase.EjecutarAsync(prestamo.Id);

            // Assert
            libro.Stock.Should().Be(stockInicial + 1);
        }

        [Fact]
        public async Task Ejecutar_DevolucionConAtraso_CreasPenalizacion()
        {
            // Arrange
            var libro = CrearLibroDisponible();
            var prestamo = new Prestamo(Guid.NewGuid(), Guid.NewGuid(), libro.Id, 1, DateTime.UtcNow.AddDays(-10));

            _prestamoRepo.Setup(r => r.GetByIdAsync(prestamo.Id)).ReturnsAsync(prestamo);
            _recursoRepo.Setup(r => r.GetByIdAsync(prestamo.RecursoId)).ReturnsAsync(libro);
            _penalizacionRepo.Setup(r => r.AddAsync(It.IsAny<Penalizacion>())).Returns(Task.CompletedTask);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _useCase.EjecutarAsync(prestamo.Id);

            // Assert
            _penalizacionRepo.Verify(r => r.AddAsync(It.IsAny<Penalizacion>()), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DevolucionSinAtraso_NoCreasPenalizacion()
        {
            // Arrange
            var libro = CrearLibroDisponible();
            var prestamo = new Prestamo(Guid.NewGuid(), Guid.NewGuid(), libro.Id, 30, DateTime.UtcNow.AddDays(-1));

            _prestamoRepo.Setup(r => r.GetByIdAsync(prestamo.Id)).ReturnsAsync(prestamo);
            _recursoRepo.Setup(r => r.GetByIdAsync(prestamo.RecursoId)).ReturnsAsync(libro);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _useCase.EjecutarAsync(prestamo.Id);

            // Assert
            _penalizacionRepo.Verify(r => r.AddAsync(It.IsAny<Penalizacion>()), Times.Never);
        }
    }
}
