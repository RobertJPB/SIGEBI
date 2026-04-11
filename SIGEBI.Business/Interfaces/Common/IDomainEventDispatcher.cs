using System.Threading.Tasks;
using SIGEBI.Domain.Common;

namespace SIGEBI.Business.Interfaces.Common
{
    /// <summary>
    /// Contract for the manual domain event dispatcher.
    /// Used when MediatR is not available.
    /// </summary>
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(IDomainEvent domainEvent);
    }

    /// <summary>
    /// Generic contract for domain event handlers.
    /// </summary>
    public interface IDomainEventHandler<in T> where T : IDomainEvent
    {
        Task HandleAsync(T domainEvent);
    }
}
