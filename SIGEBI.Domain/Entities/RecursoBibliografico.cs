using System;

namespace SIGEBI.Domain.Entities
{
    public class RecursoBibliografico
    {
        public Guid Id { get; private set; }
        public string Titulo { get; private set; }
        public string Autor { get; private set; }
        public int IdCategoria { get; private set; }
        public int Stock { get; private set; }
        public Enums.Biblioteca.EstadoRecurso Estado { get; private set; }

        private RecursoBibliografico() { }

        public RecursoBibliografico(string titulo, string autor, int idCategoria, int stockInicial)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                throw new ArgumentException("El título es obligatorio.", nameof(titulo));

            if (string.IsNullOrWhiteSpace(autor))
                throw new ArgumentException("El autor es obligatorio.", nameof(autor));

            if (idCategoria <= 0)
                throw new ArgumentException("Categoría inválida.", nameof(idCategoria));

            if (stockInicial < 0)
                throw new ArgumentOutOfRangeException(nameof(stockInicial), "El stock no puede ser negativo.");

            Id = Guid.NewGuid();
            Titulo = titulo.Trim();
            Autor = autor.Trim();
            IdCategoria = idCategoria;
            Stock = stockInicial;
            Estado = Enums.Biblioteca.EstadoRecurso.Disponible;
        }

        public void DisminuirStock()
        {
            if (Estado != Enums.Biblioteca.EstadoRecurso.Disponible)
                throw new InvalidOperationException("El recurso no está disponible.");

            if (Stock <= 0)
                throw new InvalidOperationException("No hay stock disponible.");

            Stock--;
        }

        public void AumentarStock()
        {
            Stock++;
        }

        public void Desactivar()
        {
            Estado = Enums.Biblioteca.EstadoRecurso.Inactivo;
        }

        public void Activar()
        {
            Estado = Enums.Biblioteca.EstadoRecurso.Disponible;
        }
    }
}