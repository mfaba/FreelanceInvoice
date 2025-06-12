using FreelanceInvoice.Domain.Entities;

namespace FreelanceInvoice.Infrastructure.PDF;

public interface IPdfService
{
    Task<byte[]> GenerateInvoicePdfAsync(Invoice invoice);
} 