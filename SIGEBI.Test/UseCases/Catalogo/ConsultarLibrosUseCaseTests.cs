using Microsoft.Extensions.Caching.Memory;
using FluentAssertions;
using Moq;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Business.UseCases.Catalogo;
using SIGEBI.Domain.Entities.Recursos;
using Xunit;

namespace SIGEBI.Test.UseCases.Catalogo
{
    public class ConsultarLibrosUseCaseTests
    {
        private readonly Mock<IRecursoRepository> _recursoRepo;
        private readonly Mock<IValoracionRepository> _valoracionRepo;
        private readonly IMemoryCache _cache;
        private readonly ConsultarLibrosUseCase _useCase;

        public ConsultarLibrosUseCaseTests()
        {
            _recursoRepo = new Mock<IRecursoRepository>();
            _valoracionRepo = new Mock<IValoracionRepository>();
            _cache = new MemoryCache(new MemoryCacheOptions());

            _useCase = new ConsultarLibrosUseCase(
                _recursoRepo.Object,
                _valoracionRepo.Object,
                _cache);
        }

        // -- EJECUTAR --

        [Fact]
        public async Task Ejecutar_HayRecursos_DevuelveLista()
        {
            // Arrange
            var recursos = new List<RecursoBibliografico>
            {
                new Libro("El Principito", "Antoine", 1, 5, null, "978-84-261", "Editorial X", 1943),
                new Libro("Clean Code", "Robert Martin", 1, 3, null, "978-01-323", "Editorial X", 2008)
            };

            _recursoRepo
                .Setup(r => r.GetDisponiblesAsync())
                .ReturnsAsync(recursos);

            _valoracionRepo
                .Setup(v => v.GetPromediosBatchAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(new Dictionary<Guid, double>
                {
                    { recursos[0].Id, 4.5 },
                    { recursos[1].Id, 4.5 }
                });

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
            var libro = new Libro("El Principito", "Antoine", 1, 5, null, "978-84-261", "Editorial X", 1943);

            _recursoRepo
                .Setup(r => r.GetDisponiblesAsync())
                .ReturnsAsync(new List<RecursoBibliografico> { libro });

            _valoracionRepo
                .Setup(v => v.GetPromediosBatchAsync(It.IsAny<List<Guid>>()))
                .ReturnsAsync(new Dictionary<Guid, double>
                {
                    { libro.Id, 4.8 }
                });

            // Act
            var resultado = await _useCase.EjecutarAsync();

            // Assert
            resultado.First().PromedioValoraciones.Should().Be(4.8);
        }

        // -- BUSCAR POR TITULO --

        [Fact]
        public async Task BuscarPorTitulo_TituloExistente_DevuelveResultados()
        {
            // Arrange
            var recursos = new List<RecursoBibliografico>
            {
                new Libro("El Principito", "Antoine", 1, 5, null, "978-84-261", "Editorial X", 1943)
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

        // -- BUSCAR POR CATEGORIA --

        [Fact]
        public async Task BuscarPorCategoria_CategoriaExistente_DevuelveResultados()
        {
            // Arrange
            var recursos = new List<RecursoBibliografico>
            {
                new Libro("El Principito", "Antoine", 1, 5, null, "978-84-261", "Editorial X", 1943),
                new Libro("Clean Code", "Robert Martin", 1, 3, null, "978-01-323", "Editorial X", 2008)
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
