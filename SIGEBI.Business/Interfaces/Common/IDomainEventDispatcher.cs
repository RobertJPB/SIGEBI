using System.Threading.Tasks;
using SIGEBI.Domain.Common;

namespace SIGEBI.Business.Interfaces.Common
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(IDomainEvent domainEvent);
    }
    public interface IDomainEventHandler<in T> where T : IDomainEvent
    {
        Task HandleAsync(T domainEvent);
    }
}

