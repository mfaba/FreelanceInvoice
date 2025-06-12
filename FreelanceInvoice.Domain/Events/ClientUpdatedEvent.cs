using FreelanceInvoice.Domain.Common;
using FreelanceInvoice.Domain.Entities;

namespace FreelanceInvoice.Domain.Events;

public class ClientUpdatedEvent : IDomainEvent
{
    public Client Client { get; }
    public DateTime OccurredOn { get; }

    public ClientUpdatedEvent(Client client)
    {
        Client = client;
        OccurredOn = DateTime.UtcNow;
    }
} 