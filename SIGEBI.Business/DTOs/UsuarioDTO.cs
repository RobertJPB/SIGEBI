using System;

namespace SIGEBI.Business.DTOs
{
    public class UsuarioDTO
    {
        public Guid Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Correo { get; set; } = string.Empty;

        public string Contrasena { get; set; } = string.Empty;

        public int IdRol { get; set; }

        public short Estado { get; set; }
        public string? ImagenUrl { get; set; }
        public string? MotivoSancion { get; set; }
        public DateTime? FechaFinSancion { get; set; }
    }
}
