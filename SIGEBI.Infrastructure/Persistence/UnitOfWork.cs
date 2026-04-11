using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using SIGEBI.Business.Interfaces;
using SIGEBI.Business.Interfaces.Common;
using SIGEBI.Business.Interfaces.Persistence;
using SIGEBI.Domain.Common;
using SIGEBI.Infrastructure.Persistence.Repositories;

namespace SIGEBI.Infrastructure.Persistence
{
    // Esta clase implementa el patron UnitOfWork. Su unica responsabilidad es coordinar
    // que todos los repositorios compartan el mismo contexto y guardar los cambios en una sola transaccion
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SIGEBIDbContext _context;
        private readonly IDomainEventDispatcher? _dispatcher;

        private IUsuarioRepository? _usuarios;
        private IPrestamoRepository? _prestamos;
        private IRecursoRepository? _recursos;
        private ICategoriaRepository? _categorias;
        private IPenalizacionRepository? _penalizaciones;
        private IValoracionRepository? _valoraciones;
        private INotificacionRepository? _notificaciones;
        private IAuditoriaRepository? _auditorias;
        private IListaDeseosRepository? _listasDeseos;
        private ISolicitudAccesoRepository? _solicitudesAcceso;

        public UnitOfWork(SIGEBIDbContext context, IDomainEventDispatcher? dispatcher = null)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dispatcher = dispatcher;
        }

        // Usamos Lazy loading para los repositorios
        public IUsuarioRepository Usuarios => _usuarios ??= new UsuarioRepository(_context);
        public IPrestamoRepository Prestamos => _prestamos ??= new PrestamoRepository(_context); 
        public IRecursoRepository Recursos => _recursos ??= new RecursoRepository(_context); 
        public ICategoriaRepository Categorias => _categorias ??= new CategoriaRepository(_context); 
        public IPenalizacionRepository Penalizaciones => _penalizaciones ??= new PenalizacionRepository(_context); 
        public IValoracionRepository Valoraciones => _valoraciones ??= new ValoracionRepository(_context); 
        public INotificacionRepository Notificaciones => _notificaciones ??= new NotificacionRepository(_context); 
        public IAuditoriaRepository Auditorias => _auditorias ??= new AuditoriaRepository(_context); 
        public IListaDeseosRepository ListasDeseos => _listasDeseos ??= new ListaDeseosRepository(_context); 
        public ISolicitudAccesoRepository SolicitudesAcceso => _solicitudesAcceso ??= new SolicitudAccesoRepository(_context); 

        public async Task<int> SaveChangesAsync()
        {
            // 1. Guardar cambios en la base de datos
            var result = await _context.SaveChangesAsync();

            // 2. Despachar eventos si existe un despachador
            if (_dispatcher != null)
            {
                await DispatchDomainEventsAsync();
            }

            return result;
        }

        private async Task DispatchDomainEventsAsync()
        {
            // Obtener todas las entidades rastreadas que son BaseEntity y tienen eventos
            var entitiesWithEvents = _context.ChangeTracker
                .Entries<BaseEntity>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents != null && e.DomainEvents.Any())
                .ToList();

            foreach (var entity in entitiesWithEvents)
            {
                var events = entity.DomainEvents.ToList();
                entity.ClearDomainEvents();

                foreach (var domainEvent in events)
                {
                    await _dispatcher!.DispatchAsync(domainEvent);
                }
            }
        }

        public void Dispose()
        {
            _context?.Dispose();
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
}

