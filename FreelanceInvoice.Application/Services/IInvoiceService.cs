using FreelanceInvoice.Domain.Entities;
using FreelanceInvoice.Domain.Enums;

namespace FreelanceInvoice.Application.Services;

public interface IInvoiceService
{
    Task<Invoice> CreateInvoiceAsync(
        Guid freelancerId,
        Guid clientId,
        DateTime issueDate,
        DateTime dueDate,
        string currency,
        string? notes = null);

    Task<Invoice> GetInvoiceByIdAsync(Guid id);
    Task<IEnumerable<Invoice>> GetInvoicesByFreelancerIdAsync(Guid freelancerId);
    Task<IEnumerable<Invoice>> GetInvoicesByClientIdAsync(Guid clientId);
    Task<IEnumerable<Invoice>> GetInvoicesByStatusAsync(InvoiceStatus status, Guid freelancerId);
    Task<IEnumerable<Invoice>> GetOverdueInvoicesAsync();

    Task<Invoice> AddInvoiceItemAsync(
        Guid invoiceId,
        string description,
        decimal quantity,
        decimal unitPrice,
        decimal taxRate);

    Task<Invoice> UpdateInvoiceItemAsync(
        Guid invoiceId,
        Guid itemId,
        string description,
        decimal quantity,
        decimal unitPrice,
        decimal taxRate);

    Task<Invoice> RemoveInvoiceItemAsync(Guid invoiceId, Guid itemId);
    Task<Invoice> MarkInvoiceAsSentAsync(Guid invoiceId);
    Task<Invoice> MarkInvoiceAsPaidAsync(Guid invoiceId, string paymentId, string paymentMethod);
    Task DeleteInvoiceAsync(Guid invoiceId);
} 