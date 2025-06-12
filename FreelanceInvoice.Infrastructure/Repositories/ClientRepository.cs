using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FreelanceInvoice.Domain.Entities;
using FreelanceInvoice.Domain.Repositories;
using FreelanceInvoice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FreelanceInvoice.Infrastructure.Repositories;

public class ClientRepository : BaseRepository<Client>, IClientRepository
{
    public ClientRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Client>> GetByFreelancerIdAsync(Guid freelancerId)
    {
        return await _dbSet
            .Where(c => c.FreelancerId == freelancerId)
            .ToListAsync();
    }

    public async Task<Client?> GetByEmailAsync(string email, Guid freelancerId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Email == email && c.FreelancerId == freelancerId);
    }

    public async Task<bool> ExistsByEmailAsync(string email, Guid freelancerId)
    {
        return await _dbSet
            .AnyAsync(c => c.Email == email && c.FreelancerId == freelancerId);
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Client client)
    {
        throw new NotImplementedException();
    }
} 