using System.ComponentModel.DataAnnotations;

namespace FreelanceInvoice.API.DTOs.Invoice;

public class MarkAsPaidRequest
{
    [Required]
    public string PaymentId { get; set; } = string.Empty;

    [Required]
    public string PaymentMethod { get; set; } = string.Empty;
} 