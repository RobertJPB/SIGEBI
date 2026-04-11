using FluentAssertions;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Entities.Recursos;
using SIGEBI.Domain.Enums.Biblioteca;
using SIGEBI.Domain.ValueObjects;
using Xunit;

namespace SIGEBI.Test.Domain
{
    public class LibroTests
    {
        // ── CONSTRUCTOR ──

        [Fact]
        public void Crear_LibroValido_CreaCorrectamente()
        {
            // Arrange & Act
            var libro = new Libro(Guid.NewGuid(), "El Principito", "Antoine de Saint-Exupéry", 1, 5, null,
                                  new ISBN("9781234567890"), "Editorial X", 1943);

            // Assert
            libro.Titulo.Should().Be("El Principito");
            libro.Autor.Should().Be("Antoine de Saint-Exupéry");
            libro.ISBN.Value.Should().Be("9781234567890");
            libro.Stock.Should().Be(5);
        }

        [Fact]
        public void Crear_TituloVacio_LanzaExcepcion()
        {
            var act = () => new Libro(Guid.NewGuid(), "", "Antoine", 1, 5, null, new ISBN("9781234567890"), "Editorial X", 1943);

            act.Should().Throw<ArgumentException>()
               .WithMessage("*título*");
        }

        [Fact]
        public void Crear_AutorVacio_LanzaExcepcion()
        {
            var act = () => new Libro(Guid.NewGuid(), "El Principito", "", 1, 5, null, new ISBN("9781234567890"), "Editorial X", 1943);

            act.Should().Throw<ArgumentException>()
               .WithMessage("*autor*");
        }

        [Fact]
        public void Crear_CategoriaInvalida_LanzaExcepcion()
        {
            var act = () => new Libro(Guid.NewGuid(), "El Principito", "Antoine", 0, 5, null, new ISBN("9781234567890"), "Editorial X", 1943);

            act.Should().Throw<ArgumentException>()
               .WithMessage("*ategoría*");
        }

        [Fact]
        public void Crear_EditorialVacia_LanzaExcepcion()
        {
            var act = () => new Libro(Guid.NewGuid(), "El Principito", "Antoine", 1, 5, null, new ISBN("9781234567890"), "", 1943);

            act.Should().Throw<ArgumentException>()
               .WithMessage("*editorial*");
        }

        [Fact]
        public void Crear_AnioInvalido_LanzaExcepcion()
        {
            var act = () => new Libro(Guid.NewGuid(), "El Principito", "Antoine", 1, 5, null, new ISBN("9781234567890"), "Editorial X", 0);

            act.Should().Throw<ArgumentException>()
               .WithMessage("*año*");
        }

        [Fact]
        public void Crear_StockNegativo_LanzaExcepcion()
        {
            var act = () => new Libro(Guid.NewGuid(), "El Principito", "Antoine", 1, -1, null, new ISBN("9781234567890"), "Editorial X", 1943);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        // ── STOCK ──

        [Fact]
        public void DisminuirStock_StockDisponible_DisminuyeCorrectamente()
        {
            var libro = new Libro(Guid.NewGuid(), "El Principito", "Antoine", 1, 5, null, new ISBN("9781234567890"), "Editorial X", 1943);

            libro.DisminuirStock();

            libro.Stock.Should().Be(4);
        }

        [Fact]
        public void DisminuirStock_StockCero_LanzaExcepcion()
        {
            var libro = new Libro(Guid.NewGuid(), "El Principito", "Antoine", 1, 0, null, new ISBN("9781234567890"), "Editorial X", 1943);

            var act = () => libro.DisminuirStock();

            act.Should().Throw<InvalidOperationException>();
        }

        // ── ACTUALIZAR ──

        [Fact]
        public void Actualizar_DatosValidos_ActualizaCorrectamente()
        {
            var libro = new Libro(Guid.NewGuid(), "El Principito", "Antoine", 1, 5, null, new ISBN("9781234567890"), "Editorial X", 1943);

            libro.Actualizar("Nuevo Titulo", "Nuevo Autor", 2, 10, null, new ISBN("0000000000"), "Nueva Editorial", 2000);

            libro.Titulo.Should().Be("Nuevo Titulo");
            libro.Stock.Should().Be(10);
            libro.ISBN.Value.Should().Be("0000000000");
        }

        // ── DESACTIVAR / ACTIVAR ──

        [Fact]
        public void Desactivar_LibroActivo_CambiaEstado()
        {
            var libro = new Libro(Guid.NewGuid(), "El Principito", "Antoine", 1, 5, null, new ISBN("9781234567890"), "Editorial X", 1943);

            libro.Desactivar("Motivo de prueba");

            libro.Estado.Should().Be(EstadoRecurso.Inactivo);
        }

        [Fact]
        public void DisminuirStock_LibroInactivo_LanzaExcepcion()
        {
            var libro = new Libro(Guid.NewGuid(), "El Principito", "Antoine", 1, 5, null, new ISBN("9781234567890"), "Editorial X", 1943);
            libro.Desactivar("Motivo de prueba");

            var act = () => libro.DisminuirStock();

            act.Should().Throw<InvalidOperationException>();
        }
    }
}
