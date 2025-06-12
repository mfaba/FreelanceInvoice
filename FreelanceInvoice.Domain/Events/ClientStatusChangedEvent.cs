using FreelanceInvoice.Domain.Common;
using FreelanceInvoice.Domain.Entities;

namespace FreelanceInvoice.Domain.Events;

public class ClientStatusChangedEvent : IDomainEvent
{
    public Client Client { get; }
    public bool NewStatus { get; }
    public DateTime OccurredOn { get; }

    public ClientStatusChangedEvent(Client client, bool newStatus)
    {
        Client = client;
        NewStatus = newStatus;
        OccurredOn = DateTime.UtcNow;
    }
} 