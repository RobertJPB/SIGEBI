using Microsoft.EntityFrameworkCore;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Domain.Entities;
using SIGEBI.Domain.Enums.Seguridad;
using SIGEBI.Infrastructure.Persistence.Base;

namespace SIGEBI.Infrastructure.Persistence.Repositories
{
    public class UsuarioRepository : BaseRepository<Usuario, Guid>, IUsuarioRepository
    {
        public UsuarioRepository(SIGEBIDbContext context) : base(context) { }

        public async Task<Usuario?> GetByCorreoAsync(string correo) // Buscar por correo
            => await _dbSet.FirstOrDefaultAsync(u => u.Correo == correo);

        public async Task<IEnumerable<Usuario>> GetByRolAsync(RolUsuario rol) // Filtrar por rol
            => await _dbSet.Where(u => u.Rol == rol).ToListAsync();

        public async Task<IEnumerable<Usuario>> GetActivosAsync() // Solo activos
        {
            return await _dbSet.Where(u => u.Estado == EstadoUsuario.Activo).ToListAsync();
        }
    }
}
