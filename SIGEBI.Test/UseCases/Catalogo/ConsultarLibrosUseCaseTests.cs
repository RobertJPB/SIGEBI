using FluentAssertions;
using Moq;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Business.UseCases.Catalogo;
using SIGEBI.Domain.Entities.Recursos;
using Xunit;

namespace SIGEBI.Test.UseCases.Catalogo
{
    public class ConsultarLibrosUseCaseTests
    {
        private readonly Mock<IRecursoRepository> _recursoRepo;
        private readonly Mock<IValoracionRepository> _valoracionRepo;
        private readonly ConsultarLibrosUseCase _useCase;

        public ConsultarLibrosUseCaseTests()
        {
            _recursoRepo = new Mock<IRecursoRepository>();
            _valoracionRepo = new Mock<IValoracionRepository>();

            _useCase = new ConsultarLibrosUseCase(
                _recursoRepo.Object,
                _valoracionRepo.Object);
        }

        // ── EJECUTAR ──

        [Fact]
        public async Task Ejecutar_HayRecursos_DevuelveLista()
        {
            // Arrange
            var recursos = new List<RecursoBibliografico>
            {
                new Libro("El Principito", "Antoine", 1, 5, "978-84-261", "Editorial X", 1943),
                new Libro("Clean Code", "Robert Martin", 1, 3, "978-01-323", "Editorial X", 2008)
            };

            _recursoRepo
                .Setup(r => r.GetDisponiblesAsync())
                .ReturnsAsync(recursos);

            _valoracionRepo
                .Setup(v => v.GetPromedioCalificacionAsync(It.IsAny<Guid>()))
                .ReturnsAsync(4.5);

            // Act
            var resultado = await _useCase.EjecutarAsync();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
        }

        [Fact]
        public async Task Ejecutar_NoHayRecursos_DevuelveListaVacia()
        {
            // Arrange
            _recursoRepo
                .Setup(r => r.GetDisponiblesAsync())
                .ReturnsAsync(new List<RecursoBibliografico>());

            // Act
            var resultado = await _useCase.EjecutarAsync();

            // Assert
            resultado.Should().BeEmpty();
        }

        [Fact]
        public async Task Ejecutar_DevuelvePromedioValoraciones()
        {
            // Arrange
            var libro = new Libro("El Principito", "Antoine", 1, 5, "978-84-261", "Editorial X", 1943);

            _recursoRepo
                .Setup(r => r.GetDisponiblesAsync())
                .ReturnsAsync(new List<RecursoBibliografico> { libro });

            _valoracionRepo
                .Setup(v => v.GetPromedioCalificacionAsync(libro.Id))
                .ReturnsAsync(4.8);

            // Act
            var resultado = await _useCase.EjecutarAsync();

            // Assert
            resultado.First().PromedioValoraciones.Should().Be(4.8);
        }

        // ── BUSCAR POR TITULO ──

        [Fact]
        public async Task BuscarPorTitulo_TituloExistente_DevuelveResultados()
        {
            // Arrange
            var recursos = new List<RecursoBibliografico>
            {
                new Libro("El Principito", "Antoine", 1, 5, "978-84-261", "Editorial X", 1943)
            };

            _recursoRepo
                .Setup(r => r.BuscarPorTituloAsync("Principito"))
                .ReturnsAsync(recursos);

            // Act
            var resultado = await _useCase.BuscarPorTituloAsync("Principito");

            // Assert
            resultado.Should().HaveCount(1);
            resultado.First().Titulo.Should().Be("El Principito");
        }

        [Fact]
        public async Task BuscarPorTitulo_TituloNoExistente_DevuelveListaVacia()
        {
            // Arrange
            _recursoRepo
                .Setup(r => r.BuscarPorTituloAsync("xyz"))
                .ReturnsAsync(new List<RecursoBibliografico>());

            // Act
            var resultado = await _useCase.BuscarPorTituloAsync("xyz");

            // Assert
            resultado.Should().BeEmpty();
        }

        // ── BUSCAR POR CATEGORIA ──

        [Fact]
        public async Task BuscarPorCategoria_CategoriaExistente_DevuelveResultados()
        {
            // Arrange
            var recursos = new List<RecursoBibliografico>
            {
                new Libro("El Principito", "Antoine", 1, 5, "978-84-261", "Editorial X", 1943),
                new Libro("Clean Code", "Robert Martin", 1, 3, "978-01-323", "Editorial X", 2008)
            };

            _recursoRepo
                .Setup(r => r.GetByCategoriaAsync(1))
                .ReturnsAsync(recursos);

            // Act
            var resultado = await _useCase.BuscarPorCategoriaAsync(1);

            // Assert
            resultado.Should().HaveCount(2);
        }
    }
}
