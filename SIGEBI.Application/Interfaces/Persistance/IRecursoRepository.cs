using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.Abstractions.Persistance
{
    public interface IRecursoRepository
    {
        Task<RecursoBibliografico?> GetByIdAsync(int id);
        Task<IEnumerable<RecursoBibliografico>> GetAllAsync();
        Task<IEnumerable<RecursoBibliografico>> GetByCategoriaIdAsync(int categoriaId);

        Task AddAsync(RecursoBibliografico recurso);
        void Update(RecursoBibliografico recurso);
        void Remove(RecursoBibliografico recurso);
    }
}
