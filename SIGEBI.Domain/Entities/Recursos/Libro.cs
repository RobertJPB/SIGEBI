using System;
using SIGEBI.Domain.ValueObjects;

namespace SIGEBI.Domain.Entities.Recursos
{
    // Extiende de RecursoBibliografico (OCP) y puede usarse donde se espere la base (LSP).
    public class Libro : RecursoBibliografico
    {
        public ISBN ISBN { get; private set; } = null!; // Identificador internacional
        public string Editorial { get; private set; } = null!; // Casa editorial
        public int Anio { get; private set; } // Año de publicación
        public string? Genero { get; private set; } // Género literario

        private Libro() { }

        public Libro(Guid id, string titulo, string autor, int idCategoria, int stockInicial, string? descripcion,
                     ISBN isbn, string editorial, int anio, string? genero = null)
            : base(id, titulo, autor, idCategoria, stockInicial, descripcion)
        {
            // Validaciones especificas del libro (adicionales a las del recurso base)
            if (string.IsNullOrWhiteSpace(editorial))
                throw new ArgumentException("La editorial es obligatoria.", nameof(editorial));
            if (anio <= 0)
                throw new ArgumentException("El año es inválido.", nameof(anio));

            ISBN = isbn ?? throw new ArgumentNullException(nameof(isbn));
            Editorial = editorial.Trim();
            Anio = anio;
            Genero = genero?.Trim();
        }

        public void Actualizar(string titulo, string autor, int idCategoria, int stock, string? descripcion,
                               ISBN isbn, string editorial, int anio, string? genero = null)
        {
            ActualizarDatosBase(titulo, autor, idCategoria, stock, descripcion);

            if (string.IsNullOrWhiteSpace(editorial))
                throw new ArgumentException("La editorial es obligatoria.", nameof(editorial));
            if (anio <= 0)
                throw new ArgumentException("El año es inválido.", nameof(anio));

            ISBN = isbn ?? throw new ArgumentNullException(nameof(isbn));
            Editorial = editorial.Trim();
            Anio = anio;
            Genero = genero?.Trim();
        }

        public void ActualizarGenero(string? genero) => Genero = genero?.Trim();
    }
}

