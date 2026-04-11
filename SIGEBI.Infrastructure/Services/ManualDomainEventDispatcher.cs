using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SIGEBI.Business.Interfaces.Common;
using SIGEBI.Domain.Common;

namespace SIGEBI.Infrastructure.Services
{
 
    public class ManualDomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public ManualDomainEventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task DispatchAsync(IDomainEvent domainEvent)
        {
            if (domainEvent == null) return;

            // Resolve all handlers for the specific event type
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
            var handlers = _serviceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                if (handler != null)
                {
                    // Use dynamic to call the generic HandleAsync method
                    await (Task)handler.GetType().GetMethod("HandleAsync")!.Invoke(handler, new object[] { domainEvent })!;
                }
            }
        }
    }
}
