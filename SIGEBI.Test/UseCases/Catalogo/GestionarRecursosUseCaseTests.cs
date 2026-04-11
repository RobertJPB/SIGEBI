using Microsoft.Extensions.Caching.Memory;
using FluentAssertions;
using Moq;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.UseCases.Catalogo;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Entities.Recursos;
using SIGEBI.Domain.Enums.Biblioteca;
using SIGEBI.Business.DTOs;
using SIGEBI.Business.Interfaces.Common;
using SIGEBI.Business.Interfaces.Services;
using SIGEBI.Domain.ValueObjects;
using Xunit;

namespace SIGEBI.Test.UseCases.Catalogo
{
    public class GestionarRecursosUseCaseTests
    {
        private readonly Mock<IRecursoRepository> _recursoRepo;
        private readonly Mock<IPrestamoRepository> _prestamoRepo;
        private readonly Mock<ICategoriaRepository> _categoriaRepo;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IImagenService> _imagenService;
        private readonly IMemoryCache _cache;
        private readonly Mock<IStockNotificationService> _stockNotification;
        private readonly GestionarRecursosUseCase _useCase;

        public GestionarRecursosUseCaseTests()
        {
            _recursoRepo = new Mock<IRecursoRepository>();
            _prestamoRepo = new Mock<IPrestamoRepository>();
            _categoriaRepo = new Mock<ICategoriaRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _imagenService = new Mock<IImagenService>();
            _cache = new MemoryCache(new MemoryCacheOptions());
            _stockNotification = new Mock<IStockNotificationService>();

            _useCase = new GestionarRecursosUseCase(
                _recursoRepo.Object,
                _prestamoRepo.Object,
                _categoriaRepo.Object,
                _unitOfWork.Object,
                _cache,
                new Mock<IGuidGenerator>().Object,
                _imagenService.Object,
                _stockNotification.Object);
        }

        // â”€â”€ AGREGAR LIBRO â”€â”€

        [Fact]
        public async Task AgregarLibro_CategoriaValida_DevuelveDTO()
        {
            // Arrange
            _categoriaRepo
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Categoria("FicciÃ³n"));

            _recursoRepo
                .Setup(r => r.AddAsync(It.IsAny<Libro>()))
                .Returns(Task.CompletedTask);

            _unitOfWork
                .Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var resultado = await _useCase.AgregarLibroAsync(new LibroRequestDTO
            {
                Titulo = "El Principito",
                Autor = "Antoine",
                CategoriaId = 1,
                Stock = 5,
                Descripcion = null,
                ISBN = "9781234567890",
                Editorial = "Editorial X",
                Anio = 1943
            }, Guid.NewGuid());

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
                _useCase.AgregarLibroAsync(new LibroRequestDTO
                {
                    Titulo = "El Principito",
                    Autor = "Antoine",
                    CategoriaId = 99,
                    Stock = 5,
                    ISBN = "9781234567890",
                    Editorial = "Editorial X",
                    Anio = 1943
                }, Guid.NewGuid()));
        }

        [Fact]
        public async Task AgregarLibro_ConImagen_GuardaImagenUrl()
        {
            // Arrange
            _categoriaRepo
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Categoria("FicciÃ³n"));

            _recursoRepo
                .Setup(r => r.AddAsync(It.IsAny<Libro>()))
                .Returns(Task.CompletedTask);

            _unitOfWork
                .Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            _imagenService
                .Setup(s => s.GuardarImagenAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("/imagenes/recursos/portada.jpg");

            // Act
            var resultado = await _useCase.AgregarLibroAsync(new LibroRequestDTO
            {
                Titulo = "El Principito",
                Autor = "Antoine",
                CategoriaId = 1,
                Stock = 5,
                ISBN = "9781234567890",
                Editorial = "Editorial X",
                Anio = 1943,
                ImagenStream = new MemoryStream()
            }, Guid.NewGuid());

            // Assert
            resultado.ImagenUrl.Should().Be("/imagenes/recursos/portada.jpg");
        }

        // â”€â”€ EDITAR LIBRO â”€â”€

        [Fact]
        public async Task EditarLibro_LibroExistente_ActualizaCorrectamente()
        {
            // Arrange
            var libro = new Libro(Guid.NewGuid(), "Titulo Viejo", "Autor Viejo", 1, 3, null,
                                  new ISBN("0000000000"), "Editorial Vieja", 2000);

            _recursoRepo
                .Setup(r => r.GetByIdAsync(libro.Id))
                .ReturnsAsync(libro);

            _categoriaRepo
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Categoria("FicciÃ³n"));

            _unitOfWork
                .Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var resultado = await _useCase.EditarLibroAsync(libro.Id, new LibroRequestDTO
            {
                Titulo = "Titulo Nuevo",
                Autor = "Autor Nuevo",
                CategoriaId = 1,
                Stock = 10,
                ISBN = "1111111111",
                Editorial = "Editorial Nueva",
                Anio = 2024
            });

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
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _useCase.EditarLibroAsync(Guid.NewGuid(), new LibroRequestDTO
                {
                    Titulo = "Titulo",
                    Autor = "Autor",
                    CategoriaId = 1,
                    Stock = 5,
                    ISBN = "9781234567890",
                    Editorial = "Editorial X",
                    Anio = 1943
                }));
        }

        // â”€â”€ ELIMINAR â”€â”€

        [Fact]
        public async Task EliminarRecurso_RecursoExistente_LlamadaBorradoFisico()
        {
            // Arrange
            var libro = new Libro(Guid.NewGuid(), "El Principito", "Antoine", 1, 5, null,
                                  new ISBN("9781234567890"), "Editorial X", 1943);

            _recursoRepo.Setup(r => r.GetByIdAsync(libro.Id)).ReturnsAsync(libro);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _useCase.EliminarRecursoAsync(libro.Id);

            // Assert
            _recursoRepo.Verify(r => r.Delete(libro), Times.Once);
        }

        [Fact]
        public async Task EliminarRecurso_ConPrestamosActivos_LanzaExcepcion()
        {
            // Arrange
            var libro = new Libro(Guid.NewGuid(), "El Principito", "Antoine", 1, 5, null,
                                  new ISBN("9781234567890"), "Editorial X", 1943);

            _recursoRepo.Setup(r => r.GetByIdAsync(libro.Id)).ReturnsAsync(libro);
            
            // Simulamos que hay un préstamo activo
            var prestamos = new List<Prestamo> { 
                new Prestamo(Guid.NewGuid(), Guid.NewGuid(), libro.Id, 15, DateTime.UtcNow) 
            };
            _prestamoRepo.Setup(r => r.GetActivosByRecursoIdAsync(libro.Id)).ReturnsAsync(prestamos);

            // Act & Assert
            Func<Task> action = () => _useCase.EliminarRecursoAsync(libro.Id);
            await action.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*tiene préstamos activos*");
        }

        [Fact]
        public async Task EliminarRecurso_RecursoNoEncontrado_LanzaExcepcion()
        {
            // Arrange
            _recursoRepo
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((RecursoBibliografico?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _useCase.EliminarRecursoAsync(Guid.NewGuid()));
        }
    }
}
