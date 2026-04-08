using System;
using System.Collections.Generic;
using SIGEBI.Domain.Entities.Recursos;

namespace SIGEBI.Domain.Entities
{
    public class Categoria : IDesactivable
    {
        public int Id { get; private set; } // ID numérico
        public string Nombre { get; private set; } = null!; // Nombre (Ej: Informatica)
        public Enums.Biblioteca.EstadoCategoria Estado { get; private set; } // Activa/Inactiva
        public ICollection<RecursoBibliografico> Recursos { get; private set; } = new List<RecursoBibliografico>();

        private Categoria() { }

        public Categoria(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre es obligatorio.", nameof(nombre));
            Nombre = nombre.Trim();
            Estado = Enums.Biblioteca.EstadoCategoria.Activa;
        }

        public void CambiarNombre(string nuevoNombre)
        {
            if (string.IsNullOrWhiteSpace(nuevoNombre))
                throw new ArgumentException("El nombre es obligatorio.", nameof(nuevoNombre));
            Nombre = nuevoNombre.Trim();
        }

        public void Desactivar() => Estado = Enums.Biblioteca.EstadoCategoria.Inactiva;
        public void Activar() => Estado = Enums.Biblioteca.EstadoCategoria.Activa;
    }
}
