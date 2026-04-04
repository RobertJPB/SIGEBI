using System;

namespace SIGEBI.Business.Interfaces.Common
{
    /// <summary>
    /// Interfaz para la generación de identificadores únicos.
    /// Esto permite que la capa de Business use IDs optimizados sin conocer los detalles de implementación de la base de datos.
    /// </summary>
    public interface IGuidGenerator
    {
        Guid Create();
    }
}
