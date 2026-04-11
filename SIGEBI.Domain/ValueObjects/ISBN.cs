using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SIGEBI.Domain.Common;

namespace SIGEBI.Domain.ValueObjects
{
    public class ISBN : BaseValueObject
    {
        // Validación básica de ISBN-10 o ISBN-13 (simplificada para este ejemplo,
        // pero centralizada aquí para futuras mejoras).
        private static readonly Regex ISBNRegex = new Regex(
            @"^(?=(?:\D*\d){10}(?:(?:\D*\d){3})?$)[\d-]+$",
            RegexOptions.Compiled);

        public string Value { get; private set; }

        private ISBN() { } // Para EF Core

        public ISBN(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("El ISBN no puede estar vacío.");

            // Limpiamos guiones para la validación si es necesario,
            // pero guardamos el formato original o uno estandarizado.
            string cleanISBN = value.Replace("-", "").Replace(" ", "");

            if (cleanISBN.Length != 10 && cleanISBN.Length != 13)
                throw new ArgumentException("El ISBN debe tener 10 o 13 dígitos.");

            Value = value; 
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            // Comparamos la versión limpia para igualdad structural
            yield return Value.Replace("-", "").Replace(" ", "");
        }

        public override string ToString() => Value;

        public static implicit operator string(ISBN isbn) => isbn.Value;
        public static explicit operator ISBN(string value) => new ISBN(value);
    }
}
