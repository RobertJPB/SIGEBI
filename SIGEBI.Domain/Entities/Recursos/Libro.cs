using System;
namespace SIGEBI.Domain.Entities.Recursos
{
    public class Libro : RecursoBibliografico
    {
        public string ISBN { get; private set; }
        public string Editorial { get; private set; }
        public int Anio { get; private set; }

        private Libro() { }

        public Libro(string titulo, string autor, int idCategoria, int stockInicial,
                     string isbn, string editorial, int anio)
            : base(titulo, autor, idCategoria, stockInicial)
        {
            if (string.IsNullOrWhiteSpace(isbn))
                throw new ArgumentException("El ISBN es obligatorio.", nameof(isbn));
            if (string.IsNullOrWhiteSpace(editorial))
                throw new ArgumentException("La editorial es obligatoria.", nameof(editorial));
            if (anio <= 0)
                throw new ArgumentException("El año es inválido.", nameof(anio));

            ISBN = isbn.Trim();
            Editorial = editorial.Trim();
            Anio = anio;
        }
    }
}