using System;
using System.Collections.Generic;
using System.Linq;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Entities.Recursos;
using SIGEBI.Domain.Enums.Biblioteca;
using SIGEBI.Domain.Enums.Seguridad;
using SIGEBI.Domain.Enums.Operacion;

namespace SIGEBI.Domain.DomainServices
{
    public class PrestamoPolicy
    {
        // Límites dinámicos por rol
        private const int MaxPrestamosEstudiante = 3;
        private const int MaxPrestamosDocente = 10;
        private const int DiasPlazoEstudiante = 15;
        private const int DiasPlazoDocente = 30;

        public const int MaxDiasPrestamoTotal = 30;

        public static bool PuedeRealizarPrestamo(Usuario usuario, IEnumerable<Prestamo> historialUsuario)
        {
            if (usuario.Estado != EstadoUsuario.Activo)
                return false;

            // REGLA: El acceso está condicionado por la existencia de préstamos vencidos.
            if (historialUsuario.Any(p => p.EstadoActual == EstadoPrestamo.Atrasado))
                return false;

            int limite = usuario.Rol == RolUsuario.Docente ? MaxPrestamosDocente : MaxPrestamosEstudiante;

            var activos = historialUsuario.Count(p =>
                p.EstadoActual == EstadoPrestamo.Activo ||
                p.EstadoActual == EstadoPrestamo.Atrasado);

            return activos < limite;
        }

        public static bool TienePenalizacionActiva(IEnumerable<Penalizacion> penalizaciones)
        {
            return penalizaciones.Any(p =>
                p.Estado == EstadoPenalizacion.Activa);
        }

        public static int ObtenerDiasPlazo(RolUsuario rol)
        {
            return rol == RolUsuario.Docente ? DiasPlazoDocente : DiasPlazoEstudiante;
        }

        public static void ValidarPrestamo(Usuario usuario, RecursoBibliografico recurso,
            IEnumerable<Prestamo> historialUsuario, IEnumerable<Penalizacion> penalizaciones)
        {
            // 1. Estado activo del usuario
            if (usuario.Estado != EstadoUsuario.Activo)
                throw new InvalidOperationException("El usuario debe estar en estado Activo para solicitar préstamos.");

            // 2. Existencia de préstamos vencidos
            if (historialUsuario.Any(p => p.EstadoActual == EstadoPrestamo.Atrasado))
                throw new InvalidOperationException("El usuario tiene al menos una devolución vencida y no puede realizar nuevos préstamos.");

            // 3. Existencia de penalizaciones activas
            if (TienePenalizacionActiva(penalizaciones))
                throw new InvalidOperationException("El usuario tiene una penalización activa y el acceso está restringido.");

            // 4. Límite excedido según rol
            if (!PuedeRealizarPrestamo(usuario, historialUsuario))
            {
                int limite = usuario.Rol == RolUsuario.Docente ? MaxPrestamosDocente : MaxPrestamosEstudiante;
                throw new InvalidOperationException($"El usuario ha alcanzado el límite máximo de {limite} préstamos permitidos para su rol.");
            }

            // 5. Disponibilidad del recurso
            if (recurso.Stock <= 0)
                throw new InvalidOperationException("No hay stock disponible del recurso.");
            
            if (recurso.Estado != EstadoRecurso.Disponible)
                throw new InvalidOperationException("El recurso no se encuentra en estado Disponible.");
        }
    }
}
