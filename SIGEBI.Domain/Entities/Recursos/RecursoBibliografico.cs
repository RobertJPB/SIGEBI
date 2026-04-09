using System;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Domain.Entities.Recursos
{
    public abstract class RecursoBibliografico : IDesactivable
    {
        public Guid Id { get; private set; } // ID único
        public string Titulo { get; private set; } = null!; // Título del material
        public string Autor { get; private set; } = null!; // Autor principal
        public int IdCategoria { get; private set; } // ID de categoría
        public int Stock { get; private set; } // Ejemplares disponibles
        public string? Descripcion { get; private set; } // Resumen/Descripción
        public string? ImagenUrl { get; private set; } // URL de la carátula
        public Enums.Biblioteca.EstadoRecurso Estado { get; private set; } // Estado (Disponible/Inactivo)
        public Categoria Categoria { get; private set; } = null!; // Navegación a categoría

        protected RecursoBibliografico() { }

        public RecursoBibliografico(Guid id, string titulo, string autor, int idCategoria, int stockInicial, string? descripcion)
        {
            Id = id;
            if (string.IsNullOrWhiteSpace(titulo))
                throw new ArgumentException("El título es obligatorio.", nameof(titulo));
            if (string.IsNullOrWhiteSpace(autor))
                throw new ArgumentException("El autor es obligatorio.", nameof(autor));
            if (idCategoria <= 0)
                throw new ArgumentException("Categoría inválida.", nameof(idCategoria));
            if (stockInicial < 0)
                throw new ArgumentOutOfRangeException(nameof(stockInicial), "El stock no puede ser negativo.");

            Titulo = titulo.Trim();
            Autor = autor.Trim();
            IdCategoria = idCategoria;
            Stock = stockInicial;
            Descripcion = descripcion?.Trim();
            Estado = Enums.Biblioteca.EstadoRecurso.Disponible;
        }

        public void ActualizarDatosBase(string titulo, string autor, int idCategoria, int stock, string? descripcion)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                throw new ArgumentException("El título es obligatorio.", nameof(titulo));
            if (string.IsNullOrWhiteSpace(autor))
                throw new ArgumentException("El autor es obligatorio.", nameof(autor));
            if (idCategoria <= 0)
                throw new ArgumentException("Categoría inválida.", nameof(idCategoria));
            if (stock < 0)
                throw new ArgumentOutOfRangeException(nameof(stock), "El stock no puede ser negativo.");

            Titulo = titulo.Trim();
            Autor = autor.Trim();
            IdCategoria = idCategoria;
            Stock = stock;
            Descripcion = descripcion?.Trim();
        }

        public void ActualizarImagen(string? imagenUrl) => ImagenUrl = imagenUrl;

        public void DisminuirStock()
        {
            if (Estado != Enums.Biblioteca.EstadoRecurso.Disponible)
                throw new InvalidOperationException("El recurso no está disponible.");
            
            if (Stock <= 0)
                throw new InvalidOperationException("No hay stock disponible.");
            
            Stock--;
        }

        public void AumentarStock() => Stock++;
        
        public virtual void Desactivar(string motivo) 
        {
            if (string.IsNullOrWhiteSpace(motivo)) throw new ArgumentException("El motivo es obligatorio.");
            Estado = Enums.Biblioteca.EstadoRecurso.Inactivo;
        }

        public void Activar() => Estado = Enums.Biblioteca.EstadoRecurso.Disponible;
    }
}
