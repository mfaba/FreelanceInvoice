namespace FreelanceInvoice.Infrastructure.Email;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = false);
    Task SendInvoiceEmailAsync(string to, string clientName, string invoiceNumber, string invoiceUrl);
    Task SendPaymentConfirmationEmailAsync(string to, string clientName, string invoiceNumber, decimal amount, string currency);
    Task SendInvoiceOverdueEmailAsync(string to, string clientName, string invoiceNumber, DateTime dueDate);
} 