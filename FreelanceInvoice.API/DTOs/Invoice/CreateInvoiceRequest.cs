using System.ComponentModel.DataAnnotations;

namespace FreelanceInvoice.API.DTOs.Invoice;

public class CreateInvoiceRequest
{
    [Required]
    public Guid ClientId { get; set; }

    [Required]
    public DateTime IssueDate { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    [Required]
    [StringLength(3)]
    public string Currency { get; set; } = string.Empty;

    public string? Notes { get; set; }
} 