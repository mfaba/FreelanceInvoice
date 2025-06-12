using System;
using FreelanceInvoice.Domain.Common;

namespace FreelanceInvoice.Domain.Entities;

public class InvoiceItem : Entity
{
    public Guid InvoiceId { get; private set; }
    public string Description { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TaxRate { get; private set; }
    public decimal Total => Quantity * UnitPrice;
    public decimal TaxAmount => Total * (TaxRate / 100);

    private InvoiceItem() { } // For EF Core

    public InvoiceItem(Guid invoiceId, string description, decimal quantity, decimal unitPrice, decimal taxRate)
    {
        InvoiceId = invoiceId;
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TaxRate = taxRate;
    }

    public void Update(string description, decimal quantity, decimal unitPrice, decimal taxRate)
    {
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TaxRate = taxRate;
    }
} 