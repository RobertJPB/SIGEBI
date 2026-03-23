using Microsoft.EntityFrameworkCore;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Enums.Seguridad;
using SIGEBI.Infrastructure.Persistence.Base;

namespace SIGEBI.Infrastructure.Persistence.Repositories
{
    public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(SIGEBIDbContext context) : base(context) { }

        public async Task<Usuario?> GetByCorreoAsync(string correo)
            => await _dbSet.FirstOrDefaultAsync(u => u.Correo == correo);

        public async Task<IEnumerable<Usuario>> GetByRolAsync(RolUsuario rol)
            => await _dbSet.Where(u => u.Rol == rol).ToListAsync();

        public async Task<IEnumerable<Usuario>> GetActivosAsync()
        {
            // Ojo: Solo trae los que no estan bloqueados ni dados de baja
            return await _dbSet.Where(u => u.Estado == EstadoUsuario.Activo).ToListAsync();
        }
    }
}