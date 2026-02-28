using System;

namespace SIGEBI.Domain.Entities
{
    public class Rol
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty; // Ejemplo: "Administrador", "Bibliotecario", "Estudiante"
        public string? Descripcion { get; set; }

        // Relación con Usuarios: Un rol puede pertenecer a muchos usuarios
        public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}