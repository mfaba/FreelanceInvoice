using FreelanceInvoice.Domain.Common;
using FreelanceInvoice.Domain.Entities;
using FreelanceInvoice.Domain.Enums;
using FreelanceInvoice.Domain.Repositories;
using FreelanceInvoice.Domain.Events;

namespace FreelanceInvoice.Application.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventDispatcher _eventDispatcher;

    public InvoiceService(
        IInvoiceRepository invoiceRepository,
        IClientRepository clientRepository,
        IUnitOfWork unitOfWork,
        IEventDispatcher eventDispatcher)
    {
        _invoiceRepository = invoiceRepository;
        _clientRepository = clientRepository;
        _unitOfWork = unitOfWork;
        _eventDispatcher = eventDispatcher;
    }

    public async Task<Invoice> CreateInvoiceAsync(
        Guid freelancerId,
        Guid clientId,
        DateTime issueDate,
        DateTime dueDate,
        string currency,
        string? notes = null)
    {
        // Validate client exists and belongs to freelancer
        var client = await _clientRepository.GetByIdAsync(clientId);
        if (client == null || client.FreelancerId != freelancerId)
        {
            throw new InvalidOperationException("Client not found or does not belong to the freelancer");
        }

        // Generate invoice number
        var invoiceNumber = await _invoiceRepository.GenerateInvoiceNumberAsync(freelancerId);

        // Create invoice
        var invoice = new Invoice(
            freelancerId,
            clientId,
            invoiceNumber,
            issueDate,
            dueDate,
            currency,
            notes);

        await _invoiceRepository.AddAsync(invoice);
        await _unitOfWork.SaveChangesAsync();

        // Dispatch the invoice created event
        await _eventDispatcher.DispatchAsync(new InvoiceCreatedEvent(invoice.Id, freelancerId, clientId));

        return invoice;
    }

    public async Task<Invoice> GetInvoiceByIdAsync(Guid id)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id);
        if (invoice == null)
        {
            throw new InvalidOperationException("Invoice not found");
        }
        return invoice;
    }

    public async Task<IEnumerable<Invoice>> GetInvoicesByFreelancerIdAsync(Guid freelancerId)
    {
        return await _invoiceRepository.GetByFreelancerIdAsync(freelancerId);
    }

    public async Task<IEnumerable<Invoice>> GetInvoicesByClientIdAsync(Guid clientId)
    {
        return await _invoiceRepository.GetByClientIdAsync(clientId);
    }

    public async Task<IEnumerable<Invoice>> GetInvoicesByStatusAsync(InvoiceStatus status, Guid freelancerId)
    {
        return await _invoiceRepository.GetByStatusAsync(status, freelancerId);
    }

    public async Task<IEnumerable<Invoice>> GetOverdueInvoicesAsync()
    {
        return await _invoiceRepository.GetOverdueInvoicesAsync();
    }

    public async Task<Invoice> AddInvoiceItemAsync(
        Guid invoiceId,
        string description,
        decimal quantity,
        decimal unitPrice,
        decimal taxRate)
    {
        var invoice = await GetInvoiceByIdAsync(invoiceId);
        
        invoice.AddItem(description, quantity, unitPrice, taxRate);
        await _invoiceRepository.UpdateAsync(invoice);
        await _unitOfWork.SaveChangesAsync();

        return invoice;
    }

    public async Task<Invoice> UpdateInvoiceItemAsync(
        Guid invoiceId,
        Guid itemId,
        string description,
        decimal quantity,
        decimal unitPrice,
        decimal taxRate)
    {
        var invoice = await GetInvoiceByIdAsync(invoiceId);
        
        invoice.UpdateItem(itemId, description, quantity, unitPrice, taxRate);
        await _invoiceRepository.UpdateAsync(invoice);
        await _unitOfWork.SaveChangesAsync();

        return invoice;
    }

    public async Task<Invoice> RemoveInvoiceItemAsync(Guid invoiceId, Guid itemId)
    {
        var invoice = await GetInvoiceByIdAsync(invoiceId);
        
        invoice.RemoveItem(itemId);
        await _invoiceRepository.UpdateAsync(invoice);
        await _unitOfWork.SaveChangesAsync();

        return invoice;
    }

    public async Task<Invoice> MarkInvoiceAsSentAsync(Guid invoiceId)
    {
        var invoice = await GetInvoiceByIdAsync(invoiceId);
        
        invoice.MarkAsSent();
        await _invoiceRepository.UpdateAsync(invoice);
        await _unitOfWork.SaveChangesAsync();

        return invoice;
    }

    public async Task<Invoice> MarkInvoiceAsPaidAsync(Guid invoiceId, string paymentId, string paymentMethod)
    {
        var invoice = await GetInvoiceByIdAsync(invoiceId);
        
        invoice.MarkAsPaid(paymentId, paymentMethod);
        await _invoiceRepository.UpdateAsync(invoice);
        await _unitOfWork.SaveChangesAsync();

        return invoice;
    }

    public async Task DeleteInvoiceAsync(Guid invoiceId)
    {
        var invoice = await GetInvoiceByIdAsync(invoiceId);
        await _invoiceRepository.DeleteAsync(invoiceId);
        await _unitOfWork.SaveChangesAsync();
    }
} 