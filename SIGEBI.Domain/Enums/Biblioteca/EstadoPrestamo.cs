using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGEBI.Domain.Enums.Biblioteca;

public enum EstadoPrestamo
{
    Activo = 1,
    Devuelto = 2,
    Vencido = 3,
    Atrasado = 4, 
    Cancelado = 5
}