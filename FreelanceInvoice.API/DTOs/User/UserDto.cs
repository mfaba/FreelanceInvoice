namespace FreelanceInvoice.API.DTOs.User;

public class UserDto
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string CompanyName { get; set; }
    public string? LogoPath { get; set; }
    public string? ProfilePicture { get; set; }
} 