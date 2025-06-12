using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FreelanceInvoice.Domain.Entities;

namespace FreelanceInvoice.Domain.Repositories;

public interface IClientRepository
{
    Task<Client?> GetByIdAsync(Guid id);
    Task<IEnumerable<Client>> GetByFreelancerIdAsync(Guid freelancerId);
    Task<Client?> GetByEmailAsync(string email, Guid freelancerId);
    Task AddAsync(Client client);
    Task UpdateAsync(Client client);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> ExistsByEmailAsync(string email, Guid freelancerId);
} 