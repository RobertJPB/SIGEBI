using System;
namespace SIGEBI.Domain.Entities.Recursos
{
    public class Documento : RecursoBibliografico
    {
        public string Formato { get; private set; }
        public string Institucion { get; private set; }
        public int Anio { get; private set; }

        private Documento() { }

        public Documento(string titulo, string autor, int idCategoria, int stockInicial,
                         string formato, string institucion, int anio)
            : base(titulo, autor, idCategoria, stockInicial)
        {
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