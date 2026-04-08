using SIGEBI.Domain.Entities;

namespace SIGEBI.Business.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerarToken(Usuario usuario);
    }
}
