namespace SIGEBI.Services
{
    /// <summary>
    /// Fachada que centraliza el acceso a todos los servicios de la API de SIGEBI.
    /// Sigue la recomendación arquitectónica de proveer un punto de entrada único 
    /// mientras se mantienen contratos granulares internamente.
    /// </summary>
    public interface ISigebiApiFacade
    {
        IAuthApi Auth { get; }
        IUsuariosApi Usuarios { get; }
        IRecursosApi Recursos { get; }
        ICategoriasApi Categorias { get; }
        IPrestamosApi Prestamos { get; }
        IPenalizacionesApi Penalizaciones { get; }
        INotificacionesApi Notificaciones { get; }
        IAuditoriaApi Auditoria { get; }
        IValoracionesApi Valoraciones { get; }
        IListaDeseosApi ListaDeseos { get; }
    }
}
