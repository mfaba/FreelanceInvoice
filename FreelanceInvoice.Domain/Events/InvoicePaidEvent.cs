using System;
using FreelanceInvoice.Domain.Common;

namespace FreelanceInvoice.Domain.Events;

public class InvoicePaidEvent : IDomainEvent
{
    public Guid InvoiceId { get; }
    public Guid FreelancerId { get; }
    public Guid ClientId { get; }
    public decimal Amount { get; }
    public string Currency { get; }
    public DateTime OccurredOn { get; }

    public InvoicePaidEvent(Guid invoiceId, Guid freelancerId, Guid clientId, decimal amount, string currency)
    {
        InvoiceId = invoiceId;
        FreelancerId = freelancerId;
        ClientId = clientId;
        Amount = amount;
        Currency = currency;
        OccurredOn = DateTime.UtcNow;
    }
} 