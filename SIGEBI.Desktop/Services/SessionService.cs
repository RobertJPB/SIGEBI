using SIGEBI.Domain.Enums.Seguridad;

namespace SIGEBI.Services
{
    public static class SessionService
    {
        public static string? Token { get; set; }
        public static Guid UsuarioId { get; set; }
        public static string NombreUsuario { get; set; } = string.Empty;
        public static RolUsuario Rol { get; set; }
        public static void Cerrar()
        {
            Token = null;
            UsuarioId = Guid.Empty;
            NombreUsuario = string.Empty;
            Rol = 0;
        }
    }
}


