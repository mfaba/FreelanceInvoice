using FreelanceInvoice.API.DTOs.Auth;
using FreelanceInvoice.API.DTOs.User;
using FreelanceInvoice.Domain.Repositories;
using FreelanceInvoice.Infrastructure.Authentication;
using FreelanceInvoice.Infrastructure.FileStorage;
using FreelanceInvoice.Infrastructure.Persistence;
using FreelanceInvoice.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

namespace FreelanceInvoice.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;
    private readonly IRoleService _roleService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        IJwtService jwtService,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorageService,
        IRoleService roleService,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
        _roleService = roleService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        if (await _userManager.FindByEmailAsync(request.Email) != null)
        {
            return BadRequest("User with this email already exists");
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            EmailConfirmed = true // For demo purposes, in production you should implement email verification
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        // Assign Freelancer role using the role service
        await _roleService.AssignFreelancerRoleAsync(user);

        // Create domain user
        var domainUser = new Domain.Entities.User(
            user.Id,
            request.Email,
            request.FirstName,
            request.LastName,
            request.CompanyName);

        await _userRepository.AddAsync(domainUser);
        await _unitOfWork.SaveChangesAsync();

        // Generate tokens
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Save refresh token
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            UserId = user.Id,
            Email = user.Email!,
            FirstName = request.FirstName,
            LastName = request.LastName,
            CompanyName = request.CompanyName,
            LogoPath = null
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Unauthorized("Invalid email or password");
        }

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized("Invalid email or password");
        }

        var domainUser = await _userRepository.GetByIdentityIdAsync(user.Id);
         if (domainUser == null)
        {
            return Unauthorized("User profile not found");
        }

        // Generate tokens
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Save refresh token
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            UserId = user.Id,
            Email = user.Email!,
            FirstName = domainUser.FirstName,
            LastName = domainUser.LastName,
            CompanyName = domainUser.CompanyName,
            LogoPath = domainUser.LogoPath
        };
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
        {
            return BadRequest("Refresh token is required");
        }

        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);

        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return Unauthorized("Invalid or expired refresh token");
        }

        var domainUser = await _userRepository.GetByIdAsync(user.Id);
        if (domainUser == null)
        {
            return Unauthorized("User profile not found");
        }

        // Generate new tokens
        var accessToken = _jwtService.GenerateAccessToken(user);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        // Save new refresh token
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            UserId = user.Id,
            Email = user.Email!,
            FirstName = domainUser.FirstName,
            LastName = domainUser.LastName,
            CompanyName = domainUser.CompanyName,
            LogoPath = domainUser.LogoPath
        };
    }

    [Authorize]
    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequest request)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
        {
            return BadRequest("Refresh token is required");
        }

        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);

        if (user == null)
        {
            return Unauthorized("Invalid refresh token");
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        await _userManager.UpdateAsync(user);

        return Ok();
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<ActionResult<AuthResponse>> UpdateProfile([FromForm] UpdateProfileRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound("User not found");
        }

        var domainUser = await _userRepository.GetByIdAsync(userId);
        if (domainUser == null)
        {
            return NotFound("User profile not found");
        }

        string? logoPath = null;
        if (request.Logo != null)
        {
            // Delete old logo if exists
            if (!string.IsNullOrEmpty(domainUser.LogoPath))
            {
                await _fileStorageService.DeleteFileAsync(domainUser.LogoPath);
            }

            // Save new logo
            using var memoryStream = new MemoryStream();
            await request.Logo.CopyToAsync(memoryStream);
            logoPath = await _fileStorageService.SaveFileAsync(
                memoryStream.ToArray(),
                request.Logo.FileName,
                "images/company-logos"
            );
        }

        // Update user profile
        domainUser.UpdateProfile(
            request.FirstName,
            request.LastName,
            request.CompanyName,
            logoPath ?? domainUser.LogoPath
        );

        await _unitOfWork.SaveChangesAsync();

        // Generate new tokens
        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Save refresh token
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            UserId = user.Id,
            Email = user.Email!,
            FirstName = domainUser.FirstName,
            LastName = domainUser.LastName,
            CompanyName = domainUser.CompanyName,
            LogoPath = domainUser.LogoPath
        };
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<UserDto>> GetProfile()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound("User not found");
        }

        var domainUser = await _userRepository.GetByIdentityIdAsync(userId);
        if (domainUser == null)
        {
            return NotFound("User profile not found");
        }

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = domainUser.FirstName,
            LastName = domainUser.LastName,
            CompanyName = domainUser.CompanyName,
            LogoPath = domainUser.LogoPath,
            ProfilePicture = user.ProfilePicture
        };
    }

    [HttpGet("google-login")]
    public IActionResult GoogleLogin()
    {
        _logger.LogInformation("Initiating Google login");
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action("GoogleCallback"),
            Items =
            {
                { "scheme", "Google" }
            }
        };

        return Challenge(properties, "Google");
    }

    [HttpGet("google-callback")]
    public async Task<IActionResult> GoogleCallback()
    {
        try
        {
            _logger.LogInformation("Processing Google callback");
            var result = await HttpContext.AuthenticateAsync("Google");
            if (!result.Succeeded)
            {
                _logger.LogWarning("Google authentication failed: {Error}", result.Failure?.Message);
                return BadRequest(new { error = "Google authentication failed", details = result.Failure?.Message });
            }

            var googleUser = result.Principal;
            var email = googleUser.FindFirst(ClaimTypes.Email)?.Value;
            var name = googleUser.FindFirst(ClaimTypes.Name)?.Value;
            var googleId = googleUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _logger.LogInformation("Google user info - Email: {Email}, Name: {Name}, GoogleId: {GoogleId}", 
                email, name, googleId);

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(googleId))
            {
                _logger.LogWarning("Missing required information from Google");
                return BadRequest(new { error = "Google authentication failed", details = "Missing required information from Google" });
            }

            // Check if user exists
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogInformation("Creating new user for Google account: {Email}", email);
                try
                {
                    // Create new user
                    user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true,
                        GoogleId = googleId,
                        LoginProvider = "Google",
                        ProfilePicture = googleUser.FindFirst("picture")?.Value
                    };

                    var createResult = await _userManager.CreateAsync(user);
                    if (!createResult.Succeeded)
                    {
                        _logger.LogError("Failed to create user: {Errors}", 
                            string.Join(", ", createResult.Errors.Select(e => e.Description)));
                        return BadRequest(new { error = "User creation failed", details = createResult.Errors });
                    }

                    // Create associated User entity
                    var domainUser = new Domain.Entities.User(
                        user.Id,
                        email,
                        name?.Split(' ')[0] ?? "User",
                        name?.Split(' ').Length > 1 ? string.Join(" ", name.Split(' ').Skip(1)) : "User",
                        "Company Name", // Default company name
                        user.ProfilePicture
                    );

                    await _userRepository.AddAsync(domainUser);
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogInformation("Successfully created new user and domain user");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating user");
                    return StatusCode(500, new { error = "Error creating user", details = ex.Message });
                }
            }

            try
            {
                _logger.LogInformation("Generating tokens for user: {Email}", email);
                // Generate tokens
                var accessToken = _jwtService.GenerateAccessToken(user);
                var refreshToken = _jwtService.GenerateRefreshToken();

                // Update refresh token
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                await _userManager.UpdateAsync(user);

                var domainUser = await _userRepository.GetByIdentityIdAsync(user.Id);
                if (domainUser == null)
                {
                    _logger.LogError("Domain user not found for user: {Email}", email);
                    return StatusCode(500, new { error = "Error retrieving user data" });
                }

                _logger.LogInformation("Successfully authenticated user: {Email}", email);
                return Ok(new AuthResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    User = new UserDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FirstName = domainUser.FirstName,
                        LastName = domainUser.LastName,
                        CompanyName = domainUser.CompanyName,
                        LogoPath = domainUser.LogoPath
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating tokens for user: {Email}", email);
                return StatusCode(500, new { error = "Error generating tokens", details = ex.Message });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during Google authentication");
            return StatusCode(500, new { error = "Internal server error", details = ex.Message });
        }
    }
} 