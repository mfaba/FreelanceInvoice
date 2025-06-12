using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FreelanceInvoice.Domain.Entities;
using FreelanceInvoice.Domain.Repositories;
using FreelanceInvoice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FreelanceInvoice.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        return await _context.Users.FindAsync(Guid.Parse(id));
    }
    public async Task<User?> GetByIdentityIdAsync(string id)
    {
        return await _context.Users.FirstOrDefaultAsync(w=> w.IdentityId == id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
    {
        var identityUser = await _context.Users
            .OfType<ApplicationUser>()
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (identityUser == null)
            return null;

        return await _context.Users
            .OfType<User>()
            .FirstOrDefaultAsync(u => u.IdentityId == identityUser.Id);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public void Update(User user)
    {
        _context.Users.Update(user);
    }

    public void Delete(User user)
    {
        _context.Users.Remove(user);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Users.AnyAsync(u => u.Id == id);
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
} 