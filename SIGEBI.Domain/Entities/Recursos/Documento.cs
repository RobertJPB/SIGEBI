using System;

namespace SIGEBI.Domain.Entities.Recursos
{
    // El documento hereda la logica base pero tiene sus propios datos especificos (Institucion).
    public class Documento : RecursoBibliografico
    {
        public string Formato { get; private set; } = null!; // Formato (Ej: PDF, Tesis)
        public string Institucion { get; private set; } = null!; // Institución emisora
        public int Anio { get; private set; } // Año del documento

        private Documento() { }

        public Documento(Guid id, string titulo, string autor, int idCategoria, int stockInicial, string? descripcion,
                         string formato, string institucion, int anio)
            : base(id, titulo, autor, idCategoria, stockInicial, descripcion)
        {
            // Validaciones para documentos (como tesis y manuales)
            if (string.IsNullOrWhiteSpace(formato))
                throw new ArgumentException("El formato es obligatorio.", nameof(formato));
            if (string.IsNullOrWhiteSpace(institucion))
                throw new ArgumentException("La institución es obligatoria.", nameof(institucion));
            if (anio <= 0)
                throw new ArgumentException("El año es inválido.", nameof(anio));

            Formato = formato.Trim();
            Institucion = institucion.Trim();
            Anio = anio;
        }

        public void Actualizar(string titulo, string autor, int idCategoria, int stock, string? descripcion,
                               string formato, string institucion, int anio)
        {
            ActualizarDatosBase(titulo, autor, idCategoria, stock, descripcion);

            if (string.IsNullOrWhiteSpace(formato))
                throw new ArgumentException("El formato es obligatorio.", nameof(formato));
            if (string.IsNullOrWhiteSpace(institucion))
                throw new ArgumentException("La institución es obligatoria.", nameof(institucion));
            if (anio <= 0)
                throw new ArgumentException("El año es inválido.", nameof(anio));

            Formato = formato.Trim();
            Institucion = institucion.Trim();
            Anio = anio;
        }
    }
}

