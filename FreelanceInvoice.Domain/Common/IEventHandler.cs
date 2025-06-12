using System.Threading.Tasks;

namespace FreelanceInvoice.Domain.Common;

public interface IEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent @event);
} 