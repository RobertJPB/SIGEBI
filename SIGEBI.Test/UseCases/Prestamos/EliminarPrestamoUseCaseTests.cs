using FluentAssertions;
using Moq;
using SIGEBI.Application.Interfaces;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Business.UseCases.Prestamos;
using SIGEBI.Domain.Entities;
using Xunit;

namespace SIGEBI.Test.UseCases.Prestamos
{
    public class EliminarPrestamoUseCaseTests
    {
        private readonly Mock<IPrestamoRepository> _prestamoRepo;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly EliminarPrestamoUseCase _useCase;

        public EliminarPrestamoUseCaseTests()
        {
            _prestamoRepo = new Mock<IPrestamoRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();

            _useCase = new EliminarPrestamoUseCase(
                _prestamoRepo.Object,
                _unitOfWork.Object);
        }

        // ── ELIMINAR PRESTAMO ──

        // Caso de Uso: Eliminar Préstamo - Proceso: Eliminar un préstamo existente por su ID.
        [Fact]
        public async Task Ejecutar_PrestamoExistente_EliminaYGuarda()
        {
            // Arrange
            var prestamoId = Guid.NewGuid();
            var prestamo = new Prestamo(Guid.NewGuid(), Guid.NewGuid(), 7, DateTime.UtcNow);
            _prestamoRepo.Setup(r => r.GetByIdAsync(prestamoId)).ReturnsAsync(prestamo);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _useCase.EjecutarAsync(prestamoId);

            // Assert
            _prestamoRepo.Verify(r => r.Delete(prestamo), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        // Caso de Uso: Eliminar Préstamo - Proceso: Validar que el préstamo existe antes de intentar eliminarlo.
        [Fact]
        public async Task Ejecutar_PrestamoNoExistente_LanzaExcepcion()
        {
            // Arrange
            _prestamoRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Prestamo?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _useCase.EjecutarAsync(Guid.NewGuid()));
        }
    }
}
