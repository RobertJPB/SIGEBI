using System;
using System.Threading.Tasks;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.Persistance;
using SIGEBI.Infrastructure.Persistance.Repositories;

namespace SIGEBI.Infrastructure.Persistance
{
    // Esta clase implementa el patron UnitOfWork. Su unica responsabilidad es coordinar
    // que todos los repositorios compartan el mismo contexto y guardar los cambios en una sola transaccion.
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SIGEBIDbContext _context;

        private IUsuarioRepository? _usuarios;
        private IPrestamoRepository? _prestamos;
        private IRecursoRepository? _recursos;
        private ICategoriaRepository? _categorias;
        private IPenalizacionRepository? _penalizaciones;
        private IValoracionRepository? _valoraciones;
        private INotificacionRepository? _notificaciones;
        private IAuditoriaRepository? _auditorias;
        private IListaDeseosRepository? _listasDeseos;

        public UnitOfWork(SIGEBIDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Usamos Lazy loading para los repos (este patron me lo enseño el profe)
        public IUsuarioRepository Usuarios => _usuarios ??= new UsuarioRepository(_context);
        public IPrestamoRepository Prestamos => _prestamos ??= new PrestamoRepository(_context);
        public IRecursoRepository Recursos => _recursos ??= new RecursoRepository(_context);
        public ICategoriaRepository Categorias => _categorias ??= new CategoriaRepository(_context);
        public IPenalizacionRepository Penalizaciones => _penalizaciones ??= new PenalizacionRepository(_context);
        public IValoracionRepository Valoraciones => _valoraciones ??= new ValoracionRepository(_context);
        public INotificacionRepository Notificaciones => _notificaciones ??= new NotificacionRepository(_context);
        public IAuditoriaRepository Auditorias => _auditorias ??= new AuditoriaRepository(_context);
        public IListaDeseosRepository ListasDeseos => _listasDeseos ??= new ListaDeseosRepository(_context);

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public void Dispose()
        {
            _context?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}

