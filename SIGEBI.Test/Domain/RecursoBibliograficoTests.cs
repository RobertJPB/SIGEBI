using FluentAssertions;
using SIGEBI.Domain.Entities.Recursos;
using SIGEBI.Domain.Enums.Biblioteca;
using Xunit;

namespace SIGEBI.Test.Domain
{
    public class RecursoBibliograficoTests
    {
        // â”€â”€ CONSTRUCTOR â”€â”€

        [Fact]
        public void Crear_RecursoValido_CreaCorrectamente()
        {
            var libro = new Libro(Guid.NewGuid(), "Clean Code", "Robert Martin", 1, 10, null,
                                  "978-01-323", "Prentice Hall", 2008);

            libro.Titulo.Should().Be("Clean Code");
            libro.Autor.Should().Be("Robert Martin");
            libro.Stock.Should().Be(10);
            libro.Estado.Should().Be(EstadoRecurso.Disponible);
        }

        [Fact]
        public void Crear_TituloVacio_LanzaExcepcion()
        {
            var act = () => new Libro(Guid.NewGuid(), "", "Robert Martin", 1, 10, null,
                                      "978-01-323", "Prentice Hall", 2008);

            act.Should().Throw<ArgumentException>()
               .WithMessage("*título*");
        }

        [Fact]
        public void Crear_AutorVacio_LanzaExcepcion()
        {
            var act = () => new Libro(Guid.NewGuid(), "Clean Code", "", 1, 10, null,
                                      "978-01-323", "Prentice Hall", 2008);

            act.Should().Throw<ArgumentException>()
               .WithMessage("*autor*");
        }

        [Fact]
        public void Crear_CategoriaInvalida_LanzaExcepcion()
        {
            var act = () => new Libro(Guid.NewGuid(), "Clean Code", "Robert Martin", 0, 10, null,
                                      "978-01-323", "Prentice Hall", 2008);

            act.Should().Throw<ArgumentException>()
               .WithMessage("*ategoría*");
        }

        [Fact]
        public void Crear_StockNegativo_LanzaExcepcion()
        {
            var act = () => new Libro(Guid.NewGuid(), "Clean Code", "Robert Martin", 1, -1, null,
                                      "978-01-323", "Prentice Hall", 2008);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        // â”€â”€ STOCK â”€â”€

        [Fact]
        public void DisminuirStock_StockDisponible_DisminuyeUno()
        {
            var libro = new Libro(Guid.NewGuid(), "Clean Code", "Robert Martin", 1, 3, null,
                                  "978-01-323", "Prentice Hall", 2008);

            libro.DisminuirStock();

            libro.Stock.Should().Be(2);
        }

        [Fact]
        public void AumentarStock_Siempre_AumentaUno()
        {
            var libro = new Libro(Guid.NewGuid(), "Clean Code", "Robert Martin", 1, 3, null,
                                  "978-01-323", "Prentice Hall", 2008);

            libro.AumentarStock();

            libro.Stock.Should().Be(4);
        }

        [Fact]
        public void DisminuirStock_StockCero_LanzaExcepcion()
        {
            var libro = new Libro(Guid.NewGuid(), "Clean Code", "Robert Martin", 1, 0, null,
                                  "978-01-323", "Prentice Hall", 2008);

            var act = () => libro.DisminuirStock();

            act.Should().Throw<InvalidOperationException>();
        }

        // â”€â”€ ESTADO â”€â”€

        [Fact]
        public void Desactivar_RecursoActivo_CambiaAInactivo()
        {
            var libro = new Libro(Guid.NewGuid(), "Clean Code", "Robert Martin", 1, 3, null,
                                  "978-01-323", "Prentice Hall", 2008);

            libro.Desactivar();

            libro.Estado.Should().Be(EstadoRecurso.Inactivo);
        }

        [Fact]
        public void Activar_RecursoInactivo_CambiaADisponible()
        {
            var libro = new Libro(Guid.NewGuid(), "Clean Code", "Robert Martin", 1, 3, null,
                                  "978-01-323", "Prentice Hall", 2008);
            libro.Desactivar();

            libro.Activar();

            libro.Estado.Should().Be(EstadoRecurso.Disponible);
        }

        [Fact]
        public void DisminuirStock_RecursoInactivo_LanzaExcepcion()
        {
            var libro = new Libro(Guid.NewGuid(), "Clean Code", "Robert Martin", 1, 3, null,
                                  "978-01-323", "Prentice Hall", 2008);
            libro.Desactivar();

            var act = () => libro.DisminuirStock();

            act.Should().Throw<InvalidOperationException>();
        }

        // â”€â”€ IMAGEN â”€â”€

        [Fact]
        public void ActualizarImagen_UrlValida_GuardaCorrectamente()
        {
            var libro = new Libro(Guid.NewGuid(), "Clean Code", "Robert Martin", 1, 3, null,
                                  "978-01-323", "Prentice Hall", 2008);

            libro.ActualizarImagen("/imagenes/recursos/portada.jpg");

            libro.ImagenUrl.Should().Be("/imagenes/recursos/portada.jpg");
        }

        [Fact]
        public void ActualizarImagen_UrlNula_GuardaNulo()
        {
            var libro = new Libro(Guid.NewGuid(), "Clean Code", "Robert Martin", 1, 3, null,
                                  "978-01-323", "Prentice Hall", 2008);

            libro.ActualizarImagen(null);

            libro.ImagenUrl.Should().BeNull();
        }
    }
}
