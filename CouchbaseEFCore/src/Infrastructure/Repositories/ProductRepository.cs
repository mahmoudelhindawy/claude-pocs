using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(CouchbaseContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
    {
        // Use N1QL query for efficient filtering
        var query = $@"
            SELECT META().id, * 
            FROM `products` 
            WHERE type = 'product' 
            AND category = '{category}'
        ";
        
        return await ExecuteQueryAsync(query);
    }

    public async Task<IEnumerable<Product>> GetActiveProductsAsync()
    {
        // Use N1QL query for filtering active products
        var query = @"
            SELECT META().id, * 
            FROM `products` 
            WHERE type = 'product' 
            AND isActive = true
        ";
        
        return await ExecuteQueryAsync(query);
    }

    public async Task<IEnumerable<Product>> SearchByNameAsync(string searchTerm)
    {
        // Use N1QL query with LIKE for text search
        var query = $@"
            SELECT META().id, * 
            FROM `products` 
            WHERE type = 'product' 
            AND LOWER(name) LIKE '%{searchTerm.ToLower()}%'
        ";
        
        return await ExecuteQueryAsync(query);
    }
}
