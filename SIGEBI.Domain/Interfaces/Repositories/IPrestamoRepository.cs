using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Domain.Interfaces.Repositories
{
    public interface IPrestamoRepository
    {
        Task<Prestamo?> GetByIdAsync(int id);

        Task<IEnumerable<Prestamo>> GetByUsuarioIdAsync(int usuarioId);

        Task<IEnumerable<Prestamo>> GetActivosAsync();

        Task AddAsync(Prestamo prestamo);

        void Update(Prestamo prestamo);

        void Remove(Prestamo prestamo);
    }
}