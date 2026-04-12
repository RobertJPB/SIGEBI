namespace SIGEBI.Services
{
    public class SigebiApiFacade : ISigebiApiFacade
    {
        public IAuthApi Auth { get; }
        public IUsuariosApi Usuarios { get; }
        public IRecursosApi Recursos { get; }
        public ICategoriasApi Categorias { get; }
        public IPrestamosApi Prestamos { get; }
        public IPenalizacionesApi Penalizaciones { get; }
        public INotificacionesApi Notificaciones { get; }
        public IAuditoriaApi Auditoria { get; }
        public IValoracionesApi Valoraciones { get; }
        public IListaDeseosApi ListaDeseos { get; }
        public IReportesApi Reportes { get; }

        public SigebiApiFacade(
            IAuthApi auth,
            IUsuariosApi usuarios,
            IRecursosApi recursos,
            ICategoriasApi categorias,
            IPrestamosApi prestamos,
            IPenalizacionesApi penalizaciones,
            INotificacionesApi notificaciones,
            IAuditoriaApi auditoria,
            IValoracionesApi valoraciones,
            IListaDeseosApi listaDeseos,
            IReportesApi reportes)
        {
            Auth = auth;
            Usuarios = usuarios;
            Recursos = recursos;
            Categorias = categorias;
            Prestamos = prestamos;
            Penalizaciones = penalizaciones;
            Notificaciones = notificaciones;
            Auditoria = auditoria;
            Valoraciones = valoraciones;
            ListaDeseos = listaDeseos;
            Reportes = reportes;
        }
    }
}
