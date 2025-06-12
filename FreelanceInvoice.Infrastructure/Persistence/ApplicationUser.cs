using Microsoft.AspNetCore.Identity;

namespace FreelanceInvoice.Infrastructure.Persistence;

public class ApplicationUser : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public string? GoogleId { get; set; }
    public string? ProfilePicture { get; set; }
    public string? LoginProvider { get; set; }
} 