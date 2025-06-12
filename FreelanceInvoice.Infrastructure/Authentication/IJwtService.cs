using FreelanceInvoice.Infrastructure.Persistence;
using System.Security.Claims;

namespace FreelanceInvoice.Infrastructure.Authentication;

public interface IJwtService
{
    string GenerateAccessToken(ApplicationUser user);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    bool ValidateToken(string token);
} 