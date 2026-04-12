namespace SIGEBI.Services
{
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
        IReportesApi Reportes { get; }
    }
}

