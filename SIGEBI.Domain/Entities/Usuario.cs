using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGEBI.Domain.Entities
{
  public class Usuario
        {
            public Guid Id { get; private set; }
            public string Nombre { get; private set; } = null!;
            public string Correo { get; private set; } = null!;
            public string ContrasenaHash { get; private set; } = null!;
            public int IdRol { get; private set; }
            public short Estado { get; private set; }

            private Usuario() { } // Para EF

            public Usuario(string nombre, string correo, string contrasenaHash, int idRol)
            {
                if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("El nombre es requerido.");
                if (!correo.Contains("@")) throw new ArgumentException("Correo inválido.");

                Id = Guid.NewGuid();
                Nombre = nombre;
                Correo = correo;
                ContrasenaHash = contrasenaHash;
                IdRol = idRol;
                Estado = 1; // Activo por defecto
            }

            public void BloquearUsuario() => Estado = 3;
        }
 }
