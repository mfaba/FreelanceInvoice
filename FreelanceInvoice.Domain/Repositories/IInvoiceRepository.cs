using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FreelanceInvoice.Domain.Entities;
using FreelanceInvoice.Domain.Enums;

namespace FreelanceInvoice.Domain.Repositories;

public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(Guid id);
    Task<IEnumerable<Invoice>> GetByFreelancerIdAsync(Guid freelancerId);
    Task<IEnumerable<Invoice>> GetByClientIdAsync(Guid clientId);
    Task<IEnumerable<Invoice>> GetByStatusAsync(InvoiceStatus status, Guid freelancerId);
    Task<IEnumerable<Invoice>> GetOverdueInvoicesAsync();
    Task AddAsync(Invoice invoice);
    Task UpdateAsync(Invoice invoice);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<string> GenerateInvoiceNumberAsync(Guid freelancerId);
} 