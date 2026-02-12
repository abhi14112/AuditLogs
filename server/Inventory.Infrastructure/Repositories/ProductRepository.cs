using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;
using Inventory.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetProductsByNameAsync(string name)
    {
        return await _dbSet
            .Where(p => p.Name.Contains(name))
            .ToListAsync();
    }
}
