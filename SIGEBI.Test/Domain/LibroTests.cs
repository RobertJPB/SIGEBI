using FluentAssertions;
using SIGEBI.Domain.Entities.Recursos;
using SIGEBI.Domain.Enums.Biblioteca;
using Xunit;

namespace SIGEBI.Test.Domain
{
    public class LibroTests
    {
        // â”€â”€ CONSTRUCTOR â”€â”€

        [Fact]
        public void Crear_LibroValido_CreaCorrectamente()
        {
            // Arrange & Act
            var libro = new Libro(Guid.NewGuid(), "El Principito", "Antoine de Saint-ExupÃ©ry", 1, 5, null,
                                  "978-84-261", "Editorial X", 1943);

            // Assert
            libro.Titulo.Should().Be("El Principito");
            libro.Autor.Should().Be("Antoine de Saint-ExupÃ©ry");
            libro.ISBN.Should().Be("978-84-261");
            libro.Stock.Should().Be(5);
        }

        [Fact]
        public void Crear_TituloVacio_LanzaExcepcion()
        {
            var act = () => new Libro(Guid.NewGuid(), "", "Antoine", 1, 5, null, "978-84-261", "Editorial X", 1943);

            act.Should().Throw<ArgumentException>()
               .WithMessage("*título*");
        }

        [Fact]
        public void Crear_ISBNVacio_LanzaExcepcion()
        {
            var act = () => new Libro(Guid.NewGuid(), "El Principito", "Antoine", 1, 5, null, "", "Editorial X", 1943);

            act.Should().Throw<ArgumentException>()
               .WithMessage("*ISBN*");
        }

        [Fact]
        public void Crear_EditorialVacia_LanzaExcepcion()
        {
            var act = () => new Libro(Guid.NewGuid(), "El Principito", "Antoine", 1, 5, null, "978-84-261", "", 1943);

            act.Should().Throw<ArgumentException>()
               .WithMessage("*editorial*");
        }

        [Fact]
        public void Crear_AnioInvalido_LanzaExcepcion()
        {
            var act = () => new Libro(Guid.NewGuid(), "El Principito", "Antoine", 1, 5, null, "978-84-261", "Editorial X", 0);

            act.Should().Throw<ArgumentException>()
               .WithMessage("*año*");
        }

        [Fact]
        public void Crear_StockNegativo_LanzaExcepcion()
        {
            var act = () => new Libro(Guid.NewGuid(), "El Principito", "Antoine", 1, -1, null, "978-84-261", "Editorial X", 1943);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        // â”€â”€ DISMINUIR STOCK â”€â”€

        [Fact]
        public void DisminuirStock_StockDisponible_DisminuyeCorrectamente()
        {
            var libro = new Libro(Guid.NewGuid(), "El Principito", "Antoine", 1, 5, null, "978-84-261", "Editorial X", 1943);

            libro.DisminuirStock();

            libro.Stock.Should().Be(4);
        }

        [Fact]
        public void DisminuirStock_StockCero_LanzaExcepcion()
        {
            var libro = new Libro(Guid.NewGuid(), "El Principito", "Antoine", 1, 0, null, "978-84-261", "Editorial X", 1943);

            var act = () => libro.DisminuirStock();

            act.Should().Throw<InvalidOperationException>();
        }

        // â”€â”€ ACTUALIZAR â”€â”€

        [Fact]
        public void Actualizar_DatosValidos_ActualizaCorrectamente()
        {
            var libro = new Libro(Guid.NewGuid(), "El Principito", "Antoine", 1, 5, null, "978-84-261", "Editorial X", 1943);

            libro.Actualizar("Nuevo Titulo", "Nuevo Autor", 2, 10, null, "000-00-000", "Nueva Editorial", 2000);

            libro.Titulo.Should().Be("Nuevo Titulo");
            libro.Stock.Should().Be(10);
            libro.ISBN.Should().Be("000-00-000");
        }

        // â”€â”€ DESACTIVAR / ACTIVAR â”€â”€

        [Fact]
        public void Desactivar_LibroActivo_CambiaEstado()
        {
            var libro = new Libro(Guid.NewGuid(), "El Principito", "Antoine", 1, 5, null, "978-84-261", "Editorial X", 1943);

            libro.Desactivar("Motivo de prueba");

            libro.Estado.Should().Be(EstadoRecurso.Inactivo);
        }

        [Fact]
        public void DisminuirStock_LibroInactivo_LanzaExcepcion()
        {
            var libro = new Libro(Guid.NewGuid(), "El Principito", "Antoine", 1, 5, null, "978-84-261", "Editorial X", 1943);
            libro.Desactivar("Motivo de prueba");

            var act = () => libro.DisminuirStock();

            act.Should().Throw<InvalidOperationException>();
        }
    }
}
