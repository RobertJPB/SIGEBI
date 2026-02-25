using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGEBI.Domain.Entities
{
    public class Categoria
    {
        public int Id { get; private set; } // Aquí usas int por tu SQL IDENTITY
        public string Nombre { get; private set; } = null!;
        public short Estado { get; private set; }

        private Categoria() { }

        public Categoria(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("El nombre es obligatorio.");
            Nombre = nombre;
            Estado = 1; // Activa
        }
    }
}
