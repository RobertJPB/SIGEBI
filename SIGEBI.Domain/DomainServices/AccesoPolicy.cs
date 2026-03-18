using SIGEBI.Domain.Enums.Seguridad;

namespace SIGEBI.Domain.DomainServices
{
    /// <summary>
    /// Política de Acceso Centralizada (RBAC). 
    /// Define las reglas de autorización basadas en roles en la capa de dominio,
    /// asegurando que la lógica de seguridad sea independiente de la infraestructura.
    /// </summary>
    public class AccesoPolicy
    {
        // ── CATÁLOGO ──

        public static bool PuedeVerCatalogo(RolUsuario rol)
            => true; // todos los roles pueden ver el catálogo

        public static bool PuedeGestionarRecursos(RolUsuario rol)
            => rol == RolUsuario.Administrador || rol == RolUsuario.Bibliotecario;

        // ── PRÉSTAMOS ──

        public static bool PuedeSolicitarPrestamo(RolUsuario rol)
            => rol == RolUsuario.Estudiante || rol == RolUsuario.Bibliotecario;

        public static bool PuedeGestionarPrestamos(RolUsuario rol)
            => rol == RolUsuario.Administrador || rol == RolUsuario.Bibliotecario;

        public static bool PuedeVerTodosLosPrestamos(RolUsuario rol)
            => rol == RolUsuario.Administrador || rol == RolUsuario.Bibliotecario;

        // ── USUARIOS ──

        public static bool PuedeGestionarUsuarios(RolUsuario rol)
            => rol == RolUsuario.Administrador;

        public static bool PuedeVerUsuarios(RolUsuario rol)
            => rol == RolUsuario.Administrador || rol == RolUsuario.Bibliotecario;

        // ── PENALIZACIONES ──

        public static bool PuedeGestionarPenalizaciones(RolUsuario rol)
            => rol == RolUsuario.Administrador || rol == RolUsuario.Bibliotecario;

        // ── REPORTES ──

        public static bool PuedeGenerarReportes(RolUsuario rol)
            => rol == RolUsuario.Administrador;

        // ── AUDITORÍA ──

        public static bool PuedeVerAuditoria(RolUsuario rol)
            => rol == RolUsuario.Administrador;

        // ── HELPER ──

        /// <summary>
        /// Valida si el rol tiene el permiso requerido. Si no lo tiene, lanza una excepción de tipo UnauthorizedAccessException,
        /// la cual será capturada por el ExceptionHandlingMiddleware para devolver un 403 Forbidden.
        /// </summary>
        public static void ValidarAcceso(RolUsuario rol, bool permiso, string mensaje)
        {
            if (!permiso)
                throw new UnauthorizedAccessException(
                    $"El rol {rol} no tiene permiso para realizar esta acción: {mensaje}");
        }
    }
}
