using FreelanceInvoice.API.DTOs.Invoice;
using FreelanceInvoice.Application.Services;
using FreelanceInvoice.Domain.Entities;
using FreelanceInvoice.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FreelanceInvoice.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Freelancer")]
 public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoicesController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpPost]
    public async Task<ActionResult<Invoice>> CreateInvoice(CreateInvoiceRequest request)
    {
        var freelancerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        User.IsInRole("Admin");
        var invoice = await _invoiceService.CreateInvoiceAsync(
            freelancerId,
            request.ClientId,
            request.IssueDate,
            request.DueDate,
            request.Currency,
            request.Notes);

        return CreatedAtAction(nameof(GetById), new { id = invoice.Id }, invoice);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Invoice>> GetById(Guid id)
    {
        var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
        return Ok(invoice);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Invoice>>> GetAll()
    {
        var freelancerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var invoices = await _invoiceService.GetInvoicesByFreelancerIdAsync(freelancerId);
        return Ok(invoices);
    }

    [HttpGet("client/{clientId}")]
    public async Task<ActionResult<IEnumerable<Invoice>>> GetByClientId(Guid clientId)
    {
        var invoices = await _invoiceService.GetInvoicesByClientIdAsync(clientId);
        return Ok(invoices);
    }

    [HttpGet("status/{status}")]
    public async Task<ActionResult<IEnumerable<Invoice>>> GetByStatus(InvoiceStatus status)
    {
        var freelancerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var invoices = await _invoiceService.GetInvoicesByStatusAsync(status, freelancerId);
        return Ok(invoices);
    }

    [HttpGet("overdue")]
    public async Task<ActionResult<IEnumerable<Invoice>>> GetOverdue()
    {
        var invoices = await _invoiceService.GetOverdueInvoicesAsync();
        return Ok(invoices);
    }

    [HttpPost("{id}/items")]
    public async Task<ActionResult<Invoice>> AddItem(Guid id, AddInvoiceItemRequest request)
    {
        var invoice = await _invoiceService.AddInvoiceItemAsync(
            id,
            request.Description,
            request.Quantity,
            request.UnitPrice,
            request.TaxRate);

        return Ok(invoice);
    }

    [HttpPut("{id}/items/{itemId}")]
    public async Task<ActionResult<Invoice>> UpdateItem(
        Guid id,
        Guid itemId,
        UpdateInvoiceItemRequest request)
    {
        var invoice = await _invoiceService.UpdateInvoiceItemAsync(
            id,
            itemId,
            request.Description,
            request.Quantity,
            request.UnitPrice,
            request.TaxRate);

        return Ok(invoice);
    }

    [HttpDelete("{id}/items/{itemId}")]
    public async Task<ActionResult<Invoice>> RemoveItem(Guid id, Guid itemId)
    {
        var invoice = await _invoiceService.RemoveInvoiceItemAsync(id, itemId);
        return Ok(invoice);
    }

    [HttpPost("{id}/send")]
    public async Task<ActionResult<Invoice>> MarkAsSent(Guid id)
    {
        var invoice = await _invoiceService.MarkInvoiceAsSentAsync(id);
        return Ok(invoice);
    }

    [HttpPost("{id}/pay")]
    public async Task<ActionResult<Invoice>> MarkAsPaid(Guid id, MarkAsPaidRequest request)
    {
        var invoice = await _invoiceService.MarkInvoiceAsPaidAsync(
            id,
            request.PaymentId,
            request.PaymentMethod);

        return Ok(invoice);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _invoiceService.DeleteInvoiceAsync(id);
        return NoContent();
    }
} 