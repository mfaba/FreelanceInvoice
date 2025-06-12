using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FreelanceInvoice.Domain.Entities;

namespace FreelanceInvoice.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(string id);
    Task<User?> GetByIdentityIdAsync(string id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByRefreshTokenAsync(string refreshToken);
    Task<IEnumerable<User>> GetAllAsync();
    Task AddAsync(User user);
    void Update(User user);
    void Delete(User user);
    Task<bool> ExistsAsync(Guid id);
} 