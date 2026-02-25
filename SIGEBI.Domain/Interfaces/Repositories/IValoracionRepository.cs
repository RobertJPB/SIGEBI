using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIGEBI.Domain.Entities;

namespace SIGEBI.Domain.Interfaces.Repositories
{
    public interface IValoracionRepository
    {
        Task<Valoracion?> GetByIdAsync(int id);
        Task<Valoracion?> GetByUsuarioYRecursoAsync(int usuarioId, int recursoId);
        Task<IEnumerable<Valoracion>> GetByRecursoIdAsync(int recursoId);

        Task AddAsync(Valoracion valoracion);
        void Update(Valoracion valoracion);
        void Remove(Valoracion valoracion);
    }
}