using System.Threading.Tasks;

namespace FreelanceInvoice.Domain.Repositories;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    IClientRepository Clients { get; }
    IInvoiceRepository Invoices { get; }
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
} 