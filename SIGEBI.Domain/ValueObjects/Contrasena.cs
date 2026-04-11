using System;
using System.Collections.Generic;
using SIGEBI.Domain.Common;

namespace SIGEBI.Domain.ValueObjects
{
    // vo: solo importa el valor, no tiene id (inmutable)
    public class Contrasena : BaseValueObject
    {
        public string Value { get; private set; }

        private Contrasena() { Value = string.Empty; } // para ef core

        public Contrasena(string value)
        {
            // se valida solo cuando se crea
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("La contraseña no puede estar vacía.");

            if (value.Length < 6)
                throw new ArgumentException("La contraseña debe tener al menos 6 caracteres.");

            // Aquí se podrían añadir más validaciones de complejidad
            
            Value = value;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => "********";

        // Conversiones implícitas/explícitas para facilitar el uso
        public static implicit operator string(Contrasena contrasena) => contrasena.Value;
        public static explicit operator Contrasena(string value) => new Contrasena(value);
    }
}
