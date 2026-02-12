using Inventory.Domain.Interfaces;
using Inventory.Infrastructure.Data;

namespace Inventory.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    
    public IUserRepository Users { get; }
    public IProductRepository Products { get; }
    public IAuditLogRepository AuditLogs { get; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Users = new UserRepository(_context);
        Products = new ProductRepository(_context);
        AuditLogs = new AuditLogRepository(_context);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
