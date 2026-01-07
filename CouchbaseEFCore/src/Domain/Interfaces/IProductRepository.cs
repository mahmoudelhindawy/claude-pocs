using Domain.Entities;

namespace Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    // Existing query methods
    Task<IEnumerable<Product>> GetByCategoryAsync(string category);
    Task<IEnumerable<Product>> GetActiveProductsAsync();
    Task<IEnumerable<Product>> SearchByNameAsync(string searchTerm);

    // Nested object query methods
    Task<IEnumerable<Product>> GetByWarehouseCityAsync(string city);
    Task<IEnumerable<Product>> GetByWarehouseCodeAsync(string code);
    Task<IEnumerable<Product>> GetBySpecificationAsync(string key, string value);
    Task<IEnumerable<Product>> GetNearbyProductsAsync(double latitude, double longitude, double radiusKm);
}
