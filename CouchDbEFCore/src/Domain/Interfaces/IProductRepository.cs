using Domain.Entities;

namespace Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetByCategoryAsync(string category);
    Task<IEnumerable<Product>> GetActiveProductsAsync();
}
