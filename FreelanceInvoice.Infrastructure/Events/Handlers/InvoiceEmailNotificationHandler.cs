using System.Threading.Tasks;
using FreelanceInvoice.Domain.Common;
using FreelanceInvoice.Domain.Events;
using FreelanceInvoice.Domain.Repositories;
using FreelanceInvoice.Infrastructure.Email;
using FreelanceInvoice.Infrastructure.FileStorage;

namespace FreelanceInvoice.Infrastructure.Events.Handlers;

public class InvoiceEmailNotificationHandler : IEventHandler<InvoiceCreatedEvent>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IEmailService _emailService;
    private readonly IFileStorageService _fileStorageService;

    public InvoiceEmailNotificationHandler(
        IInvoiceRepository invoiceRepository,
        IClientRepository clientRepository,
        IEmailService emailService,
        IFileStorageService fileStorageService)
    {
        _invoiceRepository = invoiceRepository;
        _clientRepository = clientRepository;
        _emailService = emailService;
        _fileStorageService = fileStorageService;
    }

    public async Task HandleAsync(InvoiceCreatedEvent @event)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(@event.InvoiceId);
        if (invoice == null) return;

        var client = await _clientRepository.GetByIdAsync(@event.ClientId);
        if (client == null) return;

        // Get the invoice URL
        var fileName = $"Invoice_{invoice.InvoiceNumber}.pdf";
        var filePath = $"invoices/{fileName}";
        var invoiceUrl = _fileStorageService.GetFileUrl(filePath);

        // Send email notification
        await _emailService.SendInvoiceEmailAsync(
            client.Email,
            client.Name,
            invoice.InvoiceNumber,
            invoiceUrl);
    }

}