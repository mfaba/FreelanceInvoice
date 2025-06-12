using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace FreelanceInvoice.Infrastructure.Email;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
    {
        using var message = new MailMessage
        {
            From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = isHtml
        };
        message.To.Add(to);

        using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
        {
            EnableSsl = _emailSettings.EnableSsl,
            Credentials = new System.Net.NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword)
        };

        await client.SendMailAsync(message);
    }

    public async Task SendInvoiceEmailAsync(string to, string clientName, string invoiceNumber, string invoiceUrl)
    {
        var subject = $"New Invoice #{invoiceNumber}";
        var body = $@"
            <h2>Dear {clientName},</h2>
            <p>A new invoice has been created for you.</p>
            <p>Invoice Number: {invoiceNumber}</p>
            <p>You can view and pay your invoice by clicking the link below:</p>
            <p><a href='{invoiceUrl}'>View Invoice</a></p>
            <p>Thank you for your business!</p>";

        await SendEmailAsync(to, subject, body, true);
    }

    public async Task SendPaymentConfirmationEmailAsync(string to, string clientName, string invoiceNumber, decimal amount, string currency)
    {
        var subject = $"Payment Confirmation - Invoice #{invoiceNumber}";
        var body = $@"
            <h2>Dear {clientName},</h2>
            <p>We have received your payment for Invoice #{invoiceNumber}.</p>
            <p>Amount Paid: {amount} {currency}</p>
            <p>Thank you for your payment!</p>";

        await SendEmailAsync(to, subject, body, true);
    }

    public async Task SendInvoiceOverdueEmailAsync(string to, string clientName, string invoiceNumber, DateTime dueDate)
    {
        var subject = $"Invoice Overdue - #{invoiceNumber}";
        var body = $@"
            <h2>Dear {clientName},</h2>
            <p>This is a reminder that Invoice #{invoiceNumber} is overdue.</p>
            <p>Due Date: {dueDate:MM/dd/yyyy}</p>
            <p>Please process the payment at your earliest convenience.</p>
            <p>If you have already made the payment, please disregard this email.</p>";

        await SendEmailAsync(to, subject, body, true);
    }
} 