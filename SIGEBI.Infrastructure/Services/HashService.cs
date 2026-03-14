using System;
using System.Security.Cryptography;
using System.Text;
using SIGEBI.Business.Interfaces.Services;

namespace SIGEBI.Infrastructure.Services
{
    public class HashService : IHashService
    {
        public string Hash(string texto)
        {
            // Ojo: Usar Bcrypt o Argon2 en vez de SHA256 en produccion real (anotado para despues)
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(texto);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public bool Verificar(string texto, string hash)
            => Hash(texto) == hash;
    }
}