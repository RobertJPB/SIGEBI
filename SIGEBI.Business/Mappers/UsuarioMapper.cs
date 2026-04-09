using SIGEBI.Business.DTOs;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.Mappers
{
    // Esta clase solo tiene una razon para existir: transcribir datos de la Entidad (BD) al DTO (Web).
    public static class UsuarioMapper
    {
        public static UsuarioDTO ToDTO(Usuario usuario)
        {
            return new UsuarioDTO
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                IdRol = (int)usuario.Rol,
                Estado = (short)usuario.Estado,
                ImagenUrl = usuario.ImagenUrl,
                MotivoSancion = ObtenerMotivo(usuario),
                FechaFinSancion = usuario.Penalizaciones?.FirstOrDefault(p => p.Estado == SIGEBI.Domain.Enums.Operacion.EstadoPenalizacion.Activa)?.FechaFin
            };
        }

        private static string? ObtenerMotivo(Usuario usuario)
        {
            if (usuario.Estado == SIGEBI.Domain.Enums.Seguridad.EstadoUsuario.Suspendido)
            {
                return usuario.Penalizaciones?.FirstOrDefault(p => p.Estado == SIGEBI.Domain.Enums.Operacion.EstadoPenalizacion.Activa)?.Motivo;
            }
            
            return usuario.MotivoEstado;
        }
    }
}
