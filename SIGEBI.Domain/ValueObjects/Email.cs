using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SIGEBI.Domain.Common;

namespace SIGEBI.Domain.ValueObjects
{
    public class Email : BaseValueObject
    {
        private static readonly Regex EmailRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string Value { get; private set; }

        private Email() { } // Para EF Core

        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El correo electrónico no puede estar vacío.");

            if (!EmailRegex.IsMatch(value))
                throw new ArgumentException("El formato del correo electrónico no es válido.");

            Value = value.ToLowerInvariant();
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;

        // Conversiones implícitas/explícitas para facilitar el uso
        public static implicit operator string(Email email) => email.Value;
        public static explicit operator Email(string value) => new Email(value);
    }
}
