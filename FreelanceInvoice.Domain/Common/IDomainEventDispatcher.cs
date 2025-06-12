using System.Threading.Tasks;

namespace FreelanceInvoice.Domain.Common;

public interface IDomainEventDispatcher
{
    Task DispatchEventsAsync(IAggregateRoot aggregateRoot);
} 