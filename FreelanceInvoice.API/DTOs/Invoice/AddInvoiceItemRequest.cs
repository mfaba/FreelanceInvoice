using System.ComponentModel.DataAnnotations;

namespace FreelanceInvoice.API.DTOs.Invoice;

public class AddInvoiceItemRequest
{
    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Quantity { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    [Required]
    [Range(0, 100)]
    public decimal TaxRate { get; set; }
} 