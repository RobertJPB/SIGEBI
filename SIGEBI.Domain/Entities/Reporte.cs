using System;
using SIGEBI.Domain.Common;
using SIGEBI.Domain.Enums.Biblioteca;

namespace SIGEBI.Domain.Entities
{
    public class Reporte : BaseEntity
    {
        public TipoReporte Tipo { get; private set; }
        public DateTime FechaGeneracion { get; private set; }
        public string Parametros { get; private set; } = null!; // JSON string
        public string Resultado { get; private set; } = null!; // JSON string
        public Guid? GeneradoPorId { get; private set; }
        public Usuario? GeneradoPor { get; private set; }

        private Reporte() { }

        public Reporte(Guid id, TipoReporte tipo, string parametros, string resultado, Guid? generadoPorId)
        {
            Id = id;
            Tipo = tipo;
            FechaGeneracion = DateTime.UtcNow;
            Parametros = parametros ?? throw new ArgumentNullException(nameof(parametros));
            Resultado = resultado ?? throw new ArgumentNullException(nameof(resultado));
            GeneradoPorId = generadoPorId;
        }
    }
}
