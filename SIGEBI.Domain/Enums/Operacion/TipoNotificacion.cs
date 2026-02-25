using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGEBI.Domain.Enums.Operacion;

public enum TipoNotificacion
{
    RecordatorioDevolucion = 1,
    AlertaVencimiento = 2,
    ConfirmacionPrestamo = 3,
    AvisoPenalizacion = 4
}