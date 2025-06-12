using System;
using FreelanceInvoice.Domain.Common;

namespace FreelanceInvoice.Domain.Entities;

public class Client : AggregateRoot
{
    public Guid FreelancerId { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PhoneNumber { get; private set; }
    public string? CompanyName { get; private set; }
    public string? Address { get; private set; }
    public string? TaxNumber { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public bool IsActive { get; private set; }

    private Client() { } // For EF Core

    public Client(Guid freelancerId, string name, string email, string phoneNumber, string? companyName = null, string? address = null, string? taxNumber = null)
    {
        FreelancerId = freelancerId;
        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;
        CompanyName = companyName;
        Address = address;
        TaxNumber = taxNumber;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public void Update(string name, string email, string phoneNumber, string? companyName = null, string? address = null, string? taxNumber = null)
    {
        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;
        CompanyName = companyName;
        Address = address;
        TaxNumber = taxNumber;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetFreelancer(Guid freelancerId)
    {
        FreelancerId = freelancerId;
    }    
}