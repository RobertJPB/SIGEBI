using System;
namespace SIGEBI.Domain.Entities.Recursos
{
    public class Revista : RecursoBibliografico
    {
        public int NumeroEdicion { get; private set; }
        public string ISSN { get; private set; }
        public DateTime FechaPublicacion { get; private set; }

        private Revista() { }

        public Revista(string titulo, string autor, int idCategoria, int stockInicial,
                       int numeroEdicion, string issn, DateTime fechaPublicacion)
            : base(titulo, autor, idCategoria, stockInicial)
        {
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