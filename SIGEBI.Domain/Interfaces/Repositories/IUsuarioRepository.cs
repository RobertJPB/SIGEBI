using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Domain.Interfaces.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByIdAsync(int id);
        Task<Usuario?> GetByCorreoAsync(string correo);
        Task<IEnumerable<Usuario>> GetAllAsync();

        Task AddAsync(Usuario usuario);
        void Update(Usuario usuario);
        void Remove(Usuario usuario);
    }
}