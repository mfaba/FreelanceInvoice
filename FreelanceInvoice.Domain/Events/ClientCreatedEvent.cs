using FreelanceInvoice.Domain.Common;
using FreelanceInvoice.Domain.Entities;

namespace FreelanceInvoice.Domain.Events;

public class ClientCreatedEvent : IDomainEvent
{
    public Client Client { get; }
    public DateTime OccurredOn { get; }

    public ClientCreatedEvent(Client client)
    {
        Client = client;
        OccurredOn = DateTime.UtcNow;
    }
} 