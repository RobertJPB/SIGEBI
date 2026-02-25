using System;

namespace SIGEBI.Domain.Entities
{
    public class Categoria
    {
        public int Id { get; private set; }
        public string Nombre { get; private set; }
        public Enums.Biblioteca.EstadoCategoria Estado { get; private set; }

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

        public void Desactivar()
        {
            Estado = Enums.Biblioteca.EstadoCategoria.Inactiva;
        }

        public void Activar()
        {
            Estado = Enums.Biblioteca.EstadoCategoria.Activa;
        }
    }
}