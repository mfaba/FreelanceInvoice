using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreelanceInvoice.Domain.Common;
using Microsoft.Extensions.DependencyInjection;

namespace FreelanceInvoice.Infrastructure.Common;

public class EventDispatcher : IEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public EventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task DispatchAsync<TEvent>(TEvent @event) where TEvent : IDomainEvent
    {
        using var scope = _serviceProvider.CreateScope();
        var handlers = scope.ServiceProvider.GetServices<IEventHandler<TEvent>>();
        
        foreach (var handler in handlers)
        {
            await handler.HandleAsync(@event);
        }
    }
} 