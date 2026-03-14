using System;

namespace SIGEBI.Domain.Entities.Recursos
{
    // Principio SOLID (OCP y LSP):
    // Extiende de RecursoBibliografico (OCP) y puede usarse donde se espere la base (LSP).
    public class Libro : RecursoBibliografico
    {
        public string ISBN { get; private set; } = null!;
        public string Editorial { get; private set; } = null!;
        public int Anio { get; private set; }
        public string? Genero { get; private set; }

        private Libro() { }

        public Libro(string titulo, string autor, int idCategoria, int stockInicial,
                     string isbn, string editorial, int anio, string? genero = null)
            : base(titulo, autor, idCategoria, stockInicial)
        {
            // Validaciones especificas del libro (adicionales a las del recurso base)
            if (string.IsNullOrWhiteSpace(isbn))
                throw new ArgumentException("El ISBN es obligatorio.", nameof(isbn));
            if (string.IsNullOrWhiteSpace(editorial))
                throw new ArgumentException("La editorial es obligatoria.", nameof(editorial));
            if (anio <= 0)
                throw new ArgumentException("El año es inválido.", nameof(anio));

            ISBN = isbn.Trim();
            Editorial = editorial.Trim();
            Anio = anio;
            Genero = genero?.Trim();
        }

        public void Actualizar(string titulo, string autor, int idCategoria, int stock,
                               string isbn, string editorial, int anio, string? genero = null)
        {
            ActualizarDatosBase(titulo, autor, idCategoria, stock);

            if (string.IsNullOrWhiteSpace(isbn))
                throw new ArgumentException("El ISBN es obligatorio.", nameof(isbn));
            if (string.IsNullOrWhiteSpace(editorial))
                throw new ArgumentException("La editorial es obligatoria.", nameof(editorial));
            if (anio <= 0)
                throw new ArgumentException("El año es inválido.", nameof(anio));

            ISBN = isbn.Trim();
            Editorial = editorial.Trim();
            Anio = anio;
            Genero = genero?.Trim();
        }

        public void ActualizarGenero(string? genero) => Genero = genero?.Trim();
    }
}
