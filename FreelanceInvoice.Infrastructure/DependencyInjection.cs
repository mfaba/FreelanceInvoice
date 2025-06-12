using FreelanceInvoice.Domain.Common;
using FreelanceInvoice.Domain.Repositories;
using FreelanceInvoice.Infrastructure.Authentication;
using FreelanceInvoice.Infrastructure.Common;
using FreelanceInvoice.Infrastructure.Email;
using FreelanceInvoice.Infrastructure.FileStorage;
using FreelanceInvoice.Infrastructure.PDF;
using FreelanceInvoice.Infrastructure.Persistence;
using FreelanceInvoice.Infrastructure.Repositories;
using FreelanceInvoice.Infrastructure.Services;
using FreelanceInvoice.Infrastructure.Events.Handlers;
using FreelanceInvoice.Domain.Events;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

namespace FreelanceInvoice.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // Add Identity
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;

            // User settings
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // Configure JWT
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                RoleClaimType = ClaimTypes.Role
            };
        });

        // Configure Email
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.AddScoped<IEmailService, EmailService>();

        // Add JWT Service
        services.AddScoped<IJwtService, JwtService>();

        // Add PDF Service
        services.AddScoped<IPdfService, PdfService>();

        // Configure File Storage
        services.Configure<FileStorageSettings>(configuration.GetSection("FileStorageSettings"));
        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        // Add Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();

        // Add Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Add Domain Event Dispatcher
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        // Add RoleService
        services.AddScoped<IRoleService, RoleService>();

        // Add Event Dispatcher
        services.AddScoped<IEventDispatcher, EventDispatcher>();

        // Add Event Handlers
        services.AddScoped<IEventHandler<InvoiceCreatedEvent>, InvoicePdfGenerationHandler>();
        services.AddScoped<IEventHandler<InvoiceCreatedEvent>, InvoiceEmailNotificationHandler>();

        services.AddScoped<IEventHandler<ClientCreatedEvent>, ClientEmailNotificationHandler>();

        return services;
    }
} 