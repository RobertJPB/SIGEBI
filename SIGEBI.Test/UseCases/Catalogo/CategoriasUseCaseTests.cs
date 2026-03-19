using Moq;
using SIGEBI.Application.Interfaces;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Business.UseCases.Catalogo;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Enums.Biblioteca;
using Xunit;

namespace SIGEBI.Test.UseCases.Catalogo
{
    public class CategoriasUseCaseTests
    {
        private readonly Mock<ICategoriaRepository> _mockRepo;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly CategoriasUseCase _useCase;

        public CategoriasUseCaseTests()
        {
            _mockRepo = new Mock<ICategoriaRepository>();
            _mockUow = new Mock<IUnitOfWork>();
            _useCase = new CategoriasUseCase(_mockRepo.Object, _mockUow.Object);
        }

        [Fact]
        public async Task CrearAsync_DebeGuardarCategoria_CuandoNombreNoExiste()
        {
            // Arrange
            string nombre = "Nueva Categoria";
            _mockRepo.Setup(r => r.GetByNombreAsync(nombre)).ReturnsAsync((Categoria?)null);

            // Act
            var result = await _useCase.CrearAsync(nombre);

            // Assert
            Assert.Equal(nombre, result.Nombre);
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Categoria>()), Times.Once);
            _mockUow.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CrearAsync_DebeLanzarExcepcion_CuandoNombreYaExiste()
        {
            // Arrange
            string nombre = "Existe";
            _mockRepo.Setup(r => r.GetByNombreAsync(nombre)).ReturnsAsync(new Categoria(nombre));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _useCase.CrearAsync(nombre));
        }

        [Fact]
        public async Task EditarAsync_DebeCambiarNombre_CuandoEsValido()
        {
            // Arrange
            int id = 1;
            string nuevoNombre = "Nombre Editado";
            var categoria = new Categoria("Original");
            _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(categoria);
            _mockRepo.Setup(r => r.GetByNombreAsync(nuevoNombre)).ReturnsAsync((Categoria?)null);

            // Act
            var result = await _useCase.EditarAsync(id, nuevoNombre);

            // Assert
            Assert.Equal(nuevoNombre, result.Nombre);
            _mockRepo.Verify(u => u.Update(categoria), Times.Once);
            _mockUow.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DesactivarAsync_DebeCambiarEstadoAInactiva()
        {
            // Arrange
            int id = 1;
            var categoria = new Categoria("Test");
            _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(categoria);

            // Act
            await _useCase.DesactivarAsync(id);

            // Assert
            Assert.Equal(EstadoCategoria.Inactiva, categoria.Estado);
            _mockRepo.Verify(u => u.Update(categoria), Times.Once);
            _mockUow.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}
