using System;
using FreelanceInvoice.Domain.Common;

namespace FreelanceInvoice.Domain.Events;

public class InvoiceOverdueEvent : IDomainEvent
{
    public Guid InvoiceId { get; }
    public Guid FreelancerId { get; }
    public Guid ClientId { get; }
    public DateTime OccurredOn { get; }

    public InvoiceOverdueEvent(Guid invoiceId, Guid freelancerId, Guid clientId)
    {
        InvoiceId = invoiceId;
        FreelancerId = freelancerId;
        ClientId = clientId;
        OccurredOn = DateTime.UtcNow;
    }
} 