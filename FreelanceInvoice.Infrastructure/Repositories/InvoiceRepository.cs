using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreelanceInvoice.Domain.Entities;
using FreelanceInvoice.Domain.Enums;
using FreelanceInvoice.Domain.Repositories;
using FreelanceInvoice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FreelanceInvoice.Infrastructure.Repositories;

public class InvoiceRepository : BaseRepository<Invoice>, IInvoiceRepository
{
    public InvoiceRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Invoice>> GetByFreelancerIdAsync(Guid freelancerId)
    {
        return await _dbSet
            .Where(i => i.FreelancerId == freelancerId)
            .Include(i => i.Items)
            .ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> GetByClientIdAsync(Guid clientId)
    {
        return await _dbSet
            .Where(i => i.ClientId == clientId)
            .Include(i => i.Items)
            .ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> GetByStatusAsync(InvoiceStatus status, Guid freelancerId)
    {
        return await _dbSet
            .Where(i => i.Status == status && i.FreelancerId == freelancerId)
            .Include(i => i.Items)
            .ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> GetOverdueInvoicesAsync()
    {
        var today = DateTime.UtcNow.Date;
        return await _dbSet
            .Where(i => i.Status == InvoiceStatus.Sent && i.DueDate < today)
            .Include(i => i.Items)
            .ToListAsync();
    }

    public async Task<string> GenerateInvoiceNumberAsync(Guid freelancerId)
    {
        var year = DateTime.UtcNow.Year;
        var month = DateTime.UtcNow.Month.ToString("D2");
        
        var lastInvoice = await _dbSet
            .Where(i => i.FreelancerId == freelancerId && 
                       i.InvoiceNumber.StartsWith($"{year}{month}"))
            .OrderByDescending(i => i.InvoiceNumber)
            .FirstOrDefaultAsync();

        if (lastInvoice == null)
        {
            return $"{year}{month}0001";
        }

        var lastNumber = int.Parse(lastInvoice.InvoiceNumber.Substring(6));
        return $"{year}{month}{(lastNumber + 1):D4}";
    }

    public override async Task<Invoice?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Invoice invoice)
    {
        throw new NotImplementedException();
    }
} 