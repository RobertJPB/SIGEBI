using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Domain.Interfaces.Repositories
{
    public interface ICategoriaRepository
    {
        Task<Categoria?> GetByIdAsync(int id);
        Task<Categoria?> GetByNombreAsync(string nombre);
        Task<IEnumerable<Categoria>> GetAllAsync();

        Task AddAsync(Categoria categoria);
        void Update(Categoria categoria);
        void Remove(Categoria categoria);
    }
}