using System;

namespace SIGEBI.Domain.Entities.Recursos
{
    // Una revista es un tipo de recurso mas. Cumple el contrato base de RecursoBibliografico.
    public class Revista : RecursoBibliografico
    {
        public int NumeroEdicion { get; private set; }
        public string ISSN { get; private set; } = null!;
        public DateTime FechaPublicacion { get; private set; }

        private Revista() { }

        public Revista(string titulo, string autor, int idCategoria, int stockInicial, string? descripcion,
                       int numeroEdicion, string issn, DateTime fechaPublicacion)
            : base(titulo, autor, idCategoria, stockInicial, descripcion)
        {
            // Validaciones especificas de la revista
            if (numeroEdicion <= 0)
                throw new ArgumentException("El número de edición es inválido.", nameof(numeroEdicion));
            if (string.IsNullOrWhiteSpace(issn))
                throw new ArgumentException("El ISSN es obligatorio.", nameof(issn));

            NumeroEdicion = numeroEdicion;
            ISSN = issn.Trim();
            FechaPublicacion = fechaPublicacion;
        }

        public void Actualizar(string titulo, string autor, int idCategoria, int stock, string? descripcion,
                               int numeroEdicion, string issn, DateTime fechaPublicacion)
        {
            ActualizarDatosBase(titulo, autor, idCategoria, stock, descripcion);

            if (numeroEdicion <= 0)
                throw new ArgumentException("El número de edición es inválido.", nameof(numeroEdicion));
            if (string.IsNullOrWhiteSpace(issn))
                throw new ArgumentException("El ISSN es obligatorio.", nameof(issn));

            NumeroEdicion = numeroEdicion;
            ISSN = issn.Trim();
            FechaPublicacion = fechaPublicacion;
        }
    }
}

