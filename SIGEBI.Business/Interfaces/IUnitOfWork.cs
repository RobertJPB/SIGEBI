using System;
using System.Threading.Tasks;
using SIGEBI.Business.Interfaces.Persistence;

namespace SIGEBI.Business.Interfaces
{
    // PATRON DE DISEÑO: Unit of Work (Unidad de Trabajo)
    // Su trabajo es englobar todos los repositorios para que compartan la misma conexion/contexto de base de datos.
    // Garantiza que cuando hacemos SaveChangesAsync(), todos los cambios de todos los repositorios se guarden como
    // una única transacción atómica (o se guarda todo, o no se guarda nada si hay error).
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        IUsuarioRepository Usuarios { get; } // Repositorio de Usuarios
        IPrestamoRepository Prestamos { get; } // Repositorio de Préstamos
        IRecursoRepository Recursos { get; } // Repositorio de Recursos
        ICategoriaRepository Categorias { get; } // Repositorio de Categorías
        IPenalizacionRepository Penalizaciones { get; } // Repositorio de Penalizaciones
        IValoracionRepository Valoraciones { get; } // Repositorio de Valoraciones
        INotificacionRepository Notificaciones { get; } // Repositorio de Notificaciones
        IAuditoriaRepository Auditorias { get; } // Repositorio de Auditoría
        IListaDeseosRepository ListasDeseos { get; } // Repositorio de Listas de Deseos
        ISolicitudAccesoRepository SolicitudesAcceso { get; } // Repositorio de Solicitudes de Acceso

        Task<int> SaveChangesAsync(); // Confirmar transacción
    }
}
