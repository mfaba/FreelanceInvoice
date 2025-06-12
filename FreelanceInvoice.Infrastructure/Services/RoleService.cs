using Microsoft.AspNetCore.Identity;
using FreelanceInvoice.Infrastructure.Persistence;

namespace FreelanceInvoice.Infrastructure.Services;

public interface IRoleService
{
    Task EnsureRolesExistAsync();
    Task AssignFreelancerRoleAsync(ApplicationUser user);
    Task<bool> IsInRoleAsync(ApplicationUser user, string role);
}

public class RoleService : IRoleService
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public RoleService(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task EnsureRolesExistAsync()
    {
        if (!await _roleManager.RoleExistsAsync("Freelancer"))
        {
            await _roleManager.CreateAsync(new IdentityRole("Freelancer"));
        }
        if (!await _roleManager.RoleExistsAsync("Admin"))
        {
            await _roleManager.CreateAsync(new IdentityRole("Admin"));
        }
    }

    public async Task AssignFreelancerRoleAsync(ApplicationUser user)
    {
        if (!await IsInRoleAsync(user, "Freelancer"))
        {
            await _userManager.AddToRoleAsync(user, "Freelancer");
        }
    }

    public async Task<bool> IsInRoleAsync(ApplicationUser user, string role)
    {
        return await _userManager.IsInRoleAsync(user, role);
    }
} 