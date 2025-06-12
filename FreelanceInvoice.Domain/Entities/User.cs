using FreelanceInvoice.Domain.Common;

namespace FreelanceInvoice.Domain.Entities;

public class User : AggregateRoot
{
    public string IdentityId { get; private set; }
    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string CompanyName { get; private set; }
    public string? LogoPath { get; private set; }
    public string? ProfilePicture { get; private set; }

    private User() { } // For EF Core

    public User(string identityId, string email, string firstName, string lastName, string companyName, string? profilePicture = null)
    {
        IdentityId = identityId;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        CompanyName = companyName;
        ProfilePicture = profilePicture;
    }

    public void UpdateProfile(string firstName, string lastName, string companyName, string? logoPath = null, string? profilePicture = null)
    {
        FirstName = firstName;
        LastName = lastName;
        CompanyName = companyName;
        LogoPath = logoPath;
        ProfilePicture = profilePicture;
    }
} 