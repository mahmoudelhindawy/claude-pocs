using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(CouchDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
    {
        // Using LINQ with EF Core patterns, but querying CouchDB
        var allProducts = await GetAllAsync();
        return allProducts
            .Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public async Task<IEnumerable<Product>> GetActiveProductsAsync()
    {
        // Using LINQ with EF Core patterns
        var allProducts = await GetAllAsync();
        return allProducts
            .Where(p => p.IsActive)
            .ToList();
    }
}
