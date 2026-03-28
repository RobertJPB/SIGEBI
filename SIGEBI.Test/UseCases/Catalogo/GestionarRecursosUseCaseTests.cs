using Microsoft.Extensions.Caching.Memory;
using FluentAssertions;
using Moq;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.UseCases.Catalogo;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Entities.Recursos;
using SIGEBI.Domain.Enums.Biblioteca;
using Xunit;

namespace SIGEBI.Test.UseCases.Catalogo
{
    public class GestionarRecursosUseCaseTests
    {
        private readonly Mock<IRecursoRepository> _recursoRepo;
        private readonly Mock<ICategoriaRepository> _categoriaRepo;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly IMemoryCache _cache;
        private readonly GestionarRecursosUseCase _useCase;

        public GestionarRecursosUseCaseTests()
        {
            _recursoRepo = new Mock<IRecursoRepository>();
            _categoriaRepo = new Mock<ICategoriaRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _cache = new MemoryCache(new MemoryCacheOptions());

            _useCase = new GestionarRecursosUseCase(
                _recursoRepo.Object,
                _categoriaRepo.Object,
                _unitOfWork.Object,
                _cache);
        }

        // ── AGREGAR LIBRO ──

        [Fact]
        public async Task AgregarLibro_CategoriaValida_DevuelveDTO()
        {
            // Arrange
            _categoriaRepo
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Categoria("Ficción"));

            _recursoRepo
                .Setup(r => r.AddAsync(It.IsAny<Libro>()))
                .Returns(Task.CompletedTask);

            _unitOfWork
                .Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var resultado = await _useCase.AgregarLibroAsync(
                "El Principito", "Antoine", 1, 5, null, "978-84-261", "Editorial X", 1943);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Titulo.Should().Be("El Principito");
            resultado.Autor.Should().Be("Antoine");
        }

        [Fact]
        public async Task AgregarLibro_CategoriaInvalida_LanzaExcepcion()
        {
            // Arrange
            _categoriaRepo
                .Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Categoria?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _useCase.AgregarLibroAsync(
                    "El Principito", "Antoine", 99, 5, null, "978-84-261", "Editorial X", 1943));
        }

        [Fact]
        public async Task AgregarLibro_ConImagen_GuardaImagenUrl()
        {
            // Arrange
            _categoriaRepo
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Categoria("Ficción"));

            _recursoRepo
                .Setup(r => r.AddAsync(It.IsAny<Libro>()))
                .Returns(Task.CompletedTask);

            _unitOfWork
                .Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var resultado = await _useCase.AgregarLibroAsync(
                "El Principito", "Antoine", 1, 5, null, "978-84-261", "Editorial X", 1943,
                imagenUrl: "/imagenes/recursos/portada.jpg");

            // Assert
            resultado.ImagenUrl.Should().Be("/imagenes/recursos/portada.jpg");
        }

        // ── EDITAR LIBRO ──

        [Fact]
        public async Task EditarLibro_LibroExistente_ActualizaCorrectamente()
        {
            // Arrange
            var libro = new Libro("Titulo Viejo", "Autor Viejo", 1, 3, null,
                                  "000-00-000", "Editorial Vieja", 2000);

            _recursoRepo
                .Setup(r => r.GetByIdAsync(libro.Id))
                .ReturnsAsync(libro);

            _categoriaRepo
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Categoria("Ficción"));

            _unitOfWork
                .Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var resultado = await _useCase.EditarLibroAsync(
                libro.Id, "Titulo Nuevo", "Autor Nuevo", 1, 10, null,
                "111-11-111", "Editorial Nueva", 2024);

            // Assert
            resultado.Titulo.Should().Be("Titulo Nuevo");
            resultado.Autor.Should().Be("Autor Nuevo");
        }

        [Fact]
        public async Task EditarLibro_LibroNoEncontrado_LanzaExcepcion()
        {
            // Arrange
            _recursoRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((RecursoBibliografico?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _useCase.EditarLibroAsync(
                    Guid.NewGuid(), "Titulo", "Autor", 1, 5, null,
                    "978-84-261", "Editorial X", 1943));
        }

        // ── ELIMINAR ──

        [Fact]
        public async Task EliminarRecurso_RecursoExistente_LlamadaBorradoFisico()
        {
            // Arrange
            var libro = new Libro("El Principito", "Antoine", 1, 5, null,
                                  "978-84-261", "Editorial X", 1943);

            _recursoRepo.Setup(r => r.GetByIdAsync(libro.Id)).ReturnsAsync(libro);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _useCase.EliminarRecursoAsync(libro.Id);

            // Assert
            _recursoRepo.Verify(r => r.Delete(libro), Times.Once);
        }

        [Fact]
        public async Task EliminarRecurso_RecursoNoEncontrado_LanzaExcepcion()
        {
            // Arrange
            _recursoRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((RecursoBibliografico?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _useCase.EliminarRecursoAsync(Guid.NewGuid()));
        }
    }
}
