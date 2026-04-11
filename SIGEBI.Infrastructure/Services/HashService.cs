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
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(texto);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public bool Verificar(string texto, string hash)
            => Hash(texto) == hash;
    }
}
