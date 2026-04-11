using System;
using System.Collections.Generic;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Entities.Recursos;
using SIGEBI.Domain.Enums.Seguridad;

namespace SIGEBI.Domain.Interfaces.Services
{
    public interface IPrestamoPolicy
    {
        void ValidarPrestamo(Usuario usuario, RecursoBibliografico recurso, IEnumerable<Prestamo> historial, IEnumerable<Penalizacion> penalizaciones);
        int ObtenerDiasPlazo(RolUsuario rol);
    }
}
