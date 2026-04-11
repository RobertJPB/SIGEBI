using System.Security.Claims;
using SIGEBI.Domain.Enums.Seguridad;

namespace SIGEBI.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        // Obtiene el rol del usuario conectado para validar sus permisos de acceso.
        public static RolUsuario ObtenerRolActual(this ClaimsPrincipal user)
        {
            var rolClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (Enum.TryParse<RolUsuario>(rolClaim, out var rol))
                return rol;
            throw new UnauthorizedAccessException("Rol no identificado en el token.");
        }

        public static Guid ObtenerIdActual(this ClaimsPrincipal user)
        {
            var idClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(idClaim, out var id))
                return id;
            throw new UnauthorizedAccessException("ID de usuario no identificado en el token.");
        }
    }
}
