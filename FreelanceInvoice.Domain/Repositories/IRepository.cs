using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FreelanceInvoice.Domain.Common;

namespace FreelanceInvoice.Domain.Repositories;

public interface IRepository<T> where T : Entity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<bool> ExistsAsync(Guid id);
} 