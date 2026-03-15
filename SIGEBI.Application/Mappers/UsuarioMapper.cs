using SIGEBI.Business.DTOs;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.Mappers
{
    // Principio SOLID (SRP - Responsabilidad Única):
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
                Estado = (short)usuario.Estado
            };
        }
    }
}