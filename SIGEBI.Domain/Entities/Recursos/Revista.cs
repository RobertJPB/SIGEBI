using System;

namespace SIGEBI.Domain.Entities.Recursos
{
    public class Revista : RecursoBibliografico
    {
        public int NumeroEdicion { get; private set; } // Número de entrega/volumen
        public string ISSN { get; private set; } = null!; // Identificador internacional de revistas
        public string? Editorial { get; private set; } // Casa editora
        public int Anio { get; private set; } // Año de publicación

        private Revista() { }

        public Revista(Guid id, string titulo, string autor, int idCategoria, int stockInicial, string? descripcion,
                       int numeroEdicion, string issn, int anio, string? editorial = null, Guid? usuarioCreadorId = null)
            : base(id, titulo, autor, idCategoria, stockInicial, descripcion, usuarioCreadorId)
        {
            // Validaciones especificas de la revista
            if (numeroEdicion <= 0)
                throw new ArgumentException("El número de edición es inválido.", nameof(numeroEdicion));
            if (string.IsNullOrWhiteSpace(issn))
                throw new ArgumentException("El ISSN es obligatorio.", nameof(issn));
            if (anio <= 0)
                throw new ArgumentException("El año es inválido.", nameof(anio));

            NumeroEdicion = numeroEdicion;
            ISSN = issn.Trim();
            Anio = anio;
            Editorial = editorial?.Trim();
        }

        public void Actualizar(string titulo, string autor, int idCategoria, int stock, string? descripcion,
                               int numeroEdicion, string issn, int anio, string? editorial = null)
        {
            ActualizarDatosBase(titulo, autor, idCategoria, stock, descripcion);

            if (numeroEdicion <= 0)
                throw new ArgumentException("El número de edición es inválido.", nameof(numeroEdicion));
            if (string.IsNullOrWhiteSpace(issn))
                throw new ArgumentException("El ISSN es obligatorio.", nameof(issn));
            if (anio <= 0)
                throw new ArgumentException("El año es inválido.", nameof(anio));

            NumeroEdicion = numeroEdicion;
            ISSN = issn.Trim();
            Anio = anio;
            Editorial = editorial?.Trim();
        }
    }
}

