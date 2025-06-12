using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreelanceInvoice.Domain.Common;
using Microsoft.Extensions.DependencyInjection;

namespace FreelanceInvoice.Infrastructure.Common;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public DomainEventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task DispatchEventsAsync(IAggregateRoot aggregateRoot)
    {
        var domainEvents = aggregateRoot.DomainEvents.ToList();
        aggregateRoot.ClearDomainEvents();

        foreach (var domainEvent in domainEvents)
        {
            var eventType = domainEvent.GetType();
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);

            using var scope = _serviceProvider.CreateScope();
            var handlers = scope.ServiceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                await ((dynamic)handler).HandleAsync((dynamic)domainEvent);
            }
        }
    }
} 