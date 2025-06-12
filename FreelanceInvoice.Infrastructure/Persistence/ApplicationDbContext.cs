using FreelanceInvoice.Domain.Entities;
using FreelanceInvoice.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FreelanceInvoice.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Client> Clients { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceItem> InvoiceItems { get; set; }
    public DbSet<Domain.Entities.User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure User
        builder.Entity<Domain.Entities.User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.IdentityId).IsRequired();
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.FirstName).IsRequired();
            entity.Property(e => e.LastName).IsRequired();
            entity.Property(e => e.CompanyName).IsRequired();

            // Configure one-to-one relationship with ApplicationUser
            entity.HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<Domain.Entities.User>(e => e.IdentityId)
                .HasPrincipalKey<ApplicationUser>(e => e.Id)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Client
        builder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Email).IsRequired();
            entity.HasOne<Domain.Entities.User>()
                .WithMany()
                .HasForeignKey(e => e.FreelancerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Invoice
        builder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.InvoiceNumber).IsRequired();
            entity.Property(e => e.IssueDate).IsRequired();
            entity.Property(e => e.DueDate).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.HasOne<Domain.Entities.User>()
                .WithMany()
                .HasForeignKey(e => e.FreelancerId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Client>()
                .WithMany()
                .HasForeignKey(e => e.ClientId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure InvoiceItem
        builder.Entity<InvoiceItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.UnitPrice).IsRequired();
            entity.HasOne<Invoice>()
                .WithMany(i => i.Items)
                .HasForeignKey(e => e.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
} 