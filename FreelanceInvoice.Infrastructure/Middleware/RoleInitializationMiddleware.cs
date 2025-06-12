using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using FreelanceInvoice.Infrastructure.Services;

namespace FreelanceInvoice.Infrastructure.Middleware;

public class RoleInitializationMiddleware
{
    private readonly RequestDelegate _next;

    public RoleInitializationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IRoleService roleService)
    {
        await roleService.EnsureRolesExistAsync();
        await _next(context);
    }
}

public static class RoleInitializationMiddlewareExtensions
{
    public static IApplicationBuilder UseRoleInitialization(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RoleInitializationMiddleware>();
    }
} 