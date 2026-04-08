namespace SIGEBI.Services
{
    /// <summary>
    /// Almacena el estado de la sesión del usuario autenticado.
    /// Se popula tras un login exitoso y se limpia al cerrar sesión.
    /// </summary>
    public static class SessionService
    {
        /// <summary>Token JWT para autenticar cada petición HTTP.</summary>
        public static string? Token { get; set; }

        /// <summary>Identificador del usuario actualmente autenticado.</summary>
        public static Guid UsuarioId { get; set; }

        /// <summary>Nombre del usuario para mostrarlo en la UI.</summary>
        public static string NombreUsuario { get; set; } = string.Empty;

        /// <summary>Limpia todos los datos de sesión (logout).</summary>
        public static void Cerrar()
        {
            Token = null;
            UsuarioId = Guid.Empty;
            NombreUsuario = string.Empty;
        }
    }
}
