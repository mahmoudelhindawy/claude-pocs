using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(CouchbaseContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
    {
        return await _dbSet
            .Where(p => p.Category == category)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetActiveProductsAsync()
    {
        return await _dbSet
            .Where(p => p.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchByNameAsync(string searchTerm)
    {
        return await _dbSet
            .Where(p => p.Name.Contains(searchTerm))
            .ToListAsync();
    }

    // ====================================================================
    // NESTED OBJECT QUERIES
    // ====================================================================

    public async Task<IEnumerable<Product>> GetByWarehouseCityAsync(string city)
    {
        return await _dbSet
            .Where(p => p.Warehouse != null && 
                        p.Warehouse.Address.City == city)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByWarehouseCodeAsync(string code)
    {
        return await _dbSet
            .Where(p => p.Warehouse != null && 
                        p.Warehouse.Code == code)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetBySpecificationAsync(string key, string value)
    {
        return await _dbSet
            .Where(p => p.Specifications.Any(s => 
                s.Key == key && 
                s.Value == value))
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetNearbyProductsAsync(
        double latitude, 
        double longitude, 
        double radiusKm)
    {
        // Simple bounding box query (for more accurate distance, use geospatial indexes)
        var latDelta = radiusKm / 111.0; // Approximate km per degree latitude
        var lonDelta = radiusKm / (111.0 * Math.Cos(latitude * Math.PI / 180.0));

        var minLat = latitude - latDelta;
        var maxLat = latitude + latDelta;
        var minLon = longitude - lonDelta;
        var maxLon = longitude + lonDelta;

        return await _dbSet
            .Where(p => p.Warehouse != null &&
                        p.Warehouse.Address.Coordinates != null &&
                        p.Warehouse.Address.Coordinates.Latitude >= minLat &&
                        p.Warehouse.Address.Coordinates.Latitude <= maxLat &&
                        p.Warehouse.Address.Coordinates.Longitude >= minLon &&
                        p.Warehouse.Address.Coordinates.Longitude <= maxLon)
            .ToListAsync();
    }
}
