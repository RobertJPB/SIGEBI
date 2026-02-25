using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGEBI.Domain.Enums.Auditoria;

public enum TipoAccionAuditoria
{
    Crear = 1,
    Actualizar = 2,
    Eliminar = 3,
    Consultar = 4,
    Login = 5,
    Logout = 6,
    PrestamoRealizado = 7,
    DevolucionRealizada = 8,
    PenalizacionAplicada = 9,
    AccesoDenegado = 10
}