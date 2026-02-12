namespace Inventory.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IProductRepository Products { get; }
    IAuditLogRepository AuditLogs { get; }
    Task<int> SaveChangesAsync();
}
