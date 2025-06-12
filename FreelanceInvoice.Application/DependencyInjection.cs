using FreelanceInvoice.Application.Interfaces;
using FreelanceInvoice.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace FreelanceInvoice.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddScoped<IClientService, ClientService>();

        return services;
    }
} 