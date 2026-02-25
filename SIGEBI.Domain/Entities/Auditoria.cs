using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SIGEBI.Domain.Enums;

namespace SIGEBI.Domain.Entities
{
    public class Auditoria
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Accion { get; set; } = string.Empty;
        public string TablaAfectada { get; set; } = string.Empty;
        public string Detalle { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public string IpAddress { get; set; } = string.Empty;

        public virtual Usuario? Usuario { get; set; }
    }
}