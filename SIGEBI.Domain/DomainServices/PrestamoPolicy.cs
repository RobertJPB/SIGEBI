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
        private const int MaxPrestamosPorEstudiante = 3;
        private const int MaxPrestamosPorBibliotecario = 5;
        private const int DiasPlazoEstudiante = 7;
        private const int DiasPlazoAdministrador = 15;
        private const int DiasPlazoAdminBibliotecario = 14;

        public static bool PuedeRealizarPrestamo(Usuario usuario, IEnumerable<Prestamo> prestamosActivos)
        {
            if (usuario.Estado != EstadoUsuario.Activo)
                return false;

            var activos = prestamosActivos.Count(p =>
                p.EstadoActual == EstadoPrestamo.Activo ||
                p.EstadoActual == EstadoPrestamo.Atrasado);

            int maxPrestamos = usuario.Rol == RolUsuario.Estudiante
                ? MaxPrestamosPorEstudiante
                : MaxPrestamosPorBibliotecario;

            return activos < maxPrestamos;
        }

        public static bool TienePenalizacionActiva(IEnumerable<Penalizacion> penalizaciones)
        {
            return penalizaciones.Any(p =>
                p.Estado == EstadoPenalizacion.Activa);
        }

        public static int ObtenerDiasPlazo(Usuario usuario)
        {
            return usuario.Rol switch
            {
                RolUsuario.Estudiante => DiasPlazoEstudiante,
                RolUsuario.Bibliotecario => DiasPlazoAdminBibliotecario,
                RolUsuario.Administrador => DiasPlazoAdministrador,
                _ => DiasPlazoEstudiante
            };
        }

        public static void ValidarPrestamo(Usuario usuario, RecursoBibliografico recurso,
            IEnumerable<Prestamo> prestamosActivos, IEnumerable<Penalizacion> penalizaciones)
        {
            if (TienePenalizacionActiva(penalizaciones))
                throw new InvalidOperationException("El usuario tiene una penalización activa y no puede realizar préstamos.");

            if (!PuedeRealizarPrestamo(usuario, prestamosActivos))
                throw new InvalidOperationException("El usuario ha alcanzado el límite de préstamos permitidos.");

            if (recurso.Estado != EstadoRecurso.Disponible)
                throw new InvalidOperationException("El recurso no está disponible para préstamo.");

            if (recurso.Stock <= 0)
                throw new InvalidOperationException("No hay stock disponible del recurso.");
        }
    }
}