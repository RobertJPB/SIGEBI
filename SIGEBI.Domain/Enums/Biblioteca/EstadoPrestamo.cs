using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGEBI.Domain.Enums.Biblioteca;

public enum EstadoPrestamo
{
    Activo = 1,
    Atrasado = 2,
    Devuelto = 3,
    Cancelado = 4
}