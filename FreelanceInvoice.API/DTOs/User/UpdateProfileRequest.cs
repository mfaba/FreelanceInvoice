using System.ComponentModel.DataAnnotations;

namespace FreelanceInvoice.API.DTOs.User;

public class UpdateProfileRequest
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public string CompanyName { get; set; } = string.Empty;

    public IFormFile? Logo { get; set; }
} 