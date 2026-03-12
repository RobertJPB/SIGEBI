using System;
using System.Threading.Tasks;
using SIGEBI.Business.Interfaces.Persistance;

namespace SIGEBI.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUsuarioRepository Usuarios { get; }
        IPrestamoRepository Prestamos { get; }
        IRecursoRepository Recursos { get; }
        ICategoriaRepository Categorias { get; }
        IPenalizacionRepository Penalizaciones { get; }
        IValoracionRepository Valoraciones { get; }
        INotificacionRepository Notificaciones { get; }
        IAuditoriaRepository Auditorias { get; }
        IListaDeseosRepository ListasDeseos { get; }

        Task<int> SaveChangesAsync();
    }
}
