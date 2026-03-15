using System;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Domain.Entities.Recursos
{
    // Principio SOLID (OCP - Abierto/Cerrado): 
    // Esta clase esta abierta a la extension (podemos crear nuevos tipos de recursos heredando de aca)
    // pero cerrada a la modificacion (no hace falta tocar esta clase para agregar un nuevo tipo).
    public class RecursoBibliografico
    {
        public Guid Id { get; private set; }
        public string Titulo { get; private set; } = null!;
        public string Autor { get; private set; } = null!;
        public int IdCategoria { get; private set; }
        public int Stock { get; private set; }
        public string? ImagenUrl { get; private set; }
        public Enums.Biblioteca.EstadoRecurso Estado { get; private set; }
        public Categoria Categoria { get; private set; } = null!;

        protected RecursoBibliografico() { }

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

        public void ActualizarDatosBase(string titulo, string autor, int idCategoria, int stock)
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
        }

        public void ActualizarImagen(string? imagenUrl) => ImagenUrl = imagenUrl;

        public void DisminuirStock()
        {
            if (Estado != Enums.Biblioteca.EstadoRecurso.Disponible)
                throw new InvalidOperationException("El recurso no está disponible.");
            
            // Si nos quedamos sin copias, tiramos error en vez de dejar stock negativo
            if (Stock <= 0)
                throw new InvalidOperationException("No hay stock disponible.");
            
            Stock--;
        }

        public void AumentarStock() => Stock++;
        public void Desactivar() => Estado = Enums.Biblioteca.EstadoRecurso.Inactivo;
        public void Activar() => Estado = Enums.Biblioteca.EstadoRecurso.Disponible;
    }
}
