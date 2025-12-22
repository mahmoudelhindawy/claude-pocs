using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using MyCouch.Requests;

namespace Infrastructure.Repositories;

public class ProductRepository : CouchDbRepository<Product>, IProductRepository
{
    public ProductRepository(CouchDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetByCategory(string category)
    {
        var allProducts = await GetAllAsync();
        return allProducts.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<IEnumerable<Product>> GetActiveProducts()
    {
        var allProducts = await GetAllAsync();
        return allProducts.Where(p => p.IsActive);
    }
}
