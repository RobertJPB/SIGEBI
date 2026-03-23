using System;
using System.Threading.Tasks;
using SIGEBI.Business.Interfaces.Persistance;

namespace SIGEBI.Business.Interfaces
{
    // PATRON DE DISEÑO: Unit of Work (Unidad de Trabajo)
    // Su trabajo es englobar todos los repositorios para que compartan la misma conexion/contexto de base de datos.
    // Garantiza que cuando hacemos SaveChangesAsync(), todos los cambios de todos los repositorios se guarden como
    // una única transacción atómica (o se guarda todo, o no se guarda nada si hay error).
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
