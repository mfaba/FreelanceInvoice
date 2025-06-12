using System.Threading.Tasks;

namespace FreelanceInvoice.Domain.Common;

public interface IEventDispatcher
{
    Task DispatchAsync<TEvent>(TEvent @event) where TEvent : IDomainEvent;
} 