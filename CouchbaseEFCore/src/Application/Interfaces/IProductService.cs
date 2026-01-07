using Application.DTOs;

namespace Application.Interfaces;

public interface IProductService
{
    // Basic CRUD
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<ProductDto?> GetByIdAsync(string id);
    Task<ProductDto> CreateAsync(CreateProductDto createDto);
    Task<ProductDto?> UpdateAsync(string id, UpdateProductDto updateDto);
    Task<bool> DeleteAsync(string id);

    // Query methods
    Task<IEnumerable<ProductDto>> GetByCategoryAsync(string category);
    Task<IEnumerable<ProductDto>> GetActiveProductsAsync();
    Task<IEnumerable<ProductDto>> SearchByNameAsync(string searchTerm);

    // Nested object queries
    Task<IEnumerable<ProductDto>> GetByWarehouseCityAsync(string city);
    Task<IEnumerable<ProductDto>> GetByWarehouseCodeAsync(string code);
    Task<IEnumerable<ProductDto>> GetBySpecificationAsync(string key, string value);
    Task<IEnumerable<ProductDto>> GetNearbyProductsAsync(double latitude, double longitude, double radiusKm);

    // Nested object operations
    Task<ProductDto?> UpdateWarehouseAsync(string id, WarehouseLocationDto warehouseDto);
    Task<ProductDto?> AddSpecificationAsync(string id, ProductSpecificationDto specificationDto);
    Task<ProductDto?> RemoveSpecificationAsync(string id, string key);
}
