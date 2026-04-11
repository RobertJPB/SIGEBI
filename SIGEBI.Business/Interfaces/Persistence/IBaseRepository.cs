using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SIGEBI.Business.Interfaces.Persistence
{
    // Esta es la interfaz base para todos los repositorios.
    // T: La entidad del dominio (ej. Usuario, Categoria)
    // TId: El tipo del identificador primario (ej. Guid, int)
    public interface IBaseRepository<T, TId> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(); // Obtener todo
        Task<T?> GetByIdAsync(TId id);      // Obtener por ID
        Task<bool> ExistsAsync(TId id);     // Verificar existencia
        Task AddAsync(T entity);            // Agregar nuevo
        void Update(T entity);              // Actualizar existente
        void Delete(T entity);              // Eliminar (Soporta borrado lógico si la entidad es IDesactivable)
        void HardDelete(T entity);          // Eliminar permanentemente de la BD
    }
}
