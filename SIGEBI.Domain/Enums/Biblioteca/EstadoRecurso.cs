using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGEBI.Domain.Enums.Biblioteca;

public enum EstadoRecurso
{
    Disponible = 1,
    Prestado = 2,
    EnMantenimiento = 3,
    Extraviado = 4,
    Reservado = 5
}