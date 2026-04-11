using System;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Common;

namespace SIGEBI.Domain.Entities.Recursos
{
    public abstract class RecursoBibliografico : BaseEntity, IDesactivable
    {
        public string Titulo { get; private set; } = null!; // Título del material
        public string Autor { get; private set; } = null!; // Autor principal
        public int IdCategoria { get; private set; } // ID de categoría
        public int Stock { get; private set; } // Ejemplares disponibles
        public string? Descripcion { get; private set; } // Resumen/Descripción
        public string? ImagenUrl { get; private set; } // URL de la carátula
        public Enums.Biblioteca.EstadoRecurso Estado { get; private set; } // Estado (Disponible/Inactivo)
        public Guid? UsuarioCreadorId { get; private set; } // id del bibliotecario
        public DateTime FechaCreacion { get; private set; } // cuando se registro
        public int? NumeroPaginas { get; private set; } // número de páginas
        public Categoria Categoria { get; private set; } = null!; // Navegación a categoría
        public Usuario? UsuarioCreador { get; private set; } // navegacion al creador

        protected RecursoBibliografico() { }

        public RecursoBibliografico(Guid id, string titulo, string autor, int idCategoria, int stockInicial, string? descripcion, Guid? usuarioCreadorId = null)
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
            UsuarioCreadorId = usuarioCreadorId;
            FechaCreacion = DateTime.UtcNow;
            NumeroPaginas = null; // Se establecerá opcionalmente o mediante sobrecarga
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

        public void CambiarNumeroPaginas(int? paginas)
        {
            if (paginas < 0) throw new ArgumentOutOfRangeException(nameof(paginas), "El número de páginas no puede ser negativo.");
            NumeroPaginas = paginas;
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
