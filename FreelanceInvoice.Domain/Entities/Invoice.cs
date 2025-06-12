using System;
using System.Collections.Generic;
using FreelanceInvoice.Domain.Common;
using FreelanceInvoice.Domain.Enums;
using FreelanceInvoice.Domain.Events;

namespace FreelanceInvoice.Domain.Entities;

public class Invoice : AggregateRoot
{
    public Guid FreelancerId { get; private set; }
    public Guid ClientId { get; private set; }
    public string InvoiceNumber { get; private set; }
    public DateTime IssueDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public string Currency { get; private set; }
    public decimal Subtotal { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal Total { get; private set; }
    public string? Notes { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public string? PaymentId { get; private set; }
    public string? PaymentMethod { get; private set; }

    private readonly List<InvoiceItem> _items = new();
    public IReadOnlyCollection<InvoiceItem> Items => _items.AsReadOnly();

    private Invoice() { } // For EF Core

    public Invoice(
        Guid freelancerId,
        Guid clientId,
        string invoiceNumber,
        DateTime issueDate,
        DateTime dueDate,
        string currency,
        string? notes = null)
    {
        FreelancerId = freelancerId;
        ClientId = clientId;
        InvoiceNumber = invoiceNumber;
        IssueDate = issueDate;
        DueDate = dueDate;
        Currency = currency;
        Notes = notes;
        Status = InvoiceStatus.Draft;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddItem(string description, decimal quantity, decimal unitPrice, decimal taxRate)
    {
        var item = new InvoiceItem(Id, description, quantity, unitPrice, taxRate);
        _items.Add(item);
        RecalculateTotals();
    }

    public void RemoveItem(Guid itemId)
    {
        var item = _items.Find(i => i.Id == itemId);
        if (item != null)
        {
            _items.Remove(item);
            RecalculateTotals();
        }
    }

    public void UpdateItem(Guid itemId, string description, decimal quantity, decimal unitPrice, decimal taxRate)
    {
        var item = _items.Find(i => i.Id == itemId);
        if (item != null)
        {
            item.Update(description, quantity, unitPrice, taxRate);
            RecalculateTotals();
        }
    }

    private void RecalculateTotals()
    {
        Subtotal = _items.Sum(item => item.Total);
        TaxAmount = _items.Sum(item => item.TaxAmount);
        Total = Subtotal + TaxAmount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsSent()
    {
        if (Status != InvoiceStatus.Draft)
            throw new InvalidOperationException("Only draft invoices can be marked as sent.");

        Status = InvoiceStatus.Sent;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new InvoiceSentEvent(Id, FreelancerId, ClientId));
    }

    public void MarkAsPaid(string paymentId, string paymentMethod)
    {
        if (Status != InvoiceStatus.Sent)
            throw new InvalidOperationException("Only sent invoices can be marked as paid.");

        Status = InvoiceStatus.Paid;
        PaymentId = paymentId;
        PaymentMethod = paymentMethod;
        PaidAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new InvoicePaidEvent(Id, FreelancerId, ClientId, Total, Currency));
    }

    public void MarkAsOverdue()
    {
        if (Status != InvoiceStatus.Sent)
            throw new InvalidOperationException("Only sent invoices can be marked as overdue.");

        Status = InvoiceStatus.Overdue;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new InvoiceOverdueEvent(Id, FreelancerId, ClientId));
    }

    public void UpdateNotes(string notes)
    {
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }
} 