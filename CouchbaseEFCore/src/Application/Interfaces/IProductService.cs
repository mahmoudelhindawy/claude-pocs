using Application.DTOs;

namespace Application.Interfaces;

public interface IProductService
{
    Task<ProductDto?> GetProductByIdAsync(string id);
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(string category);
    Task<IEnumerable<ProductDto>> GetActiveProductsAsync();
    Task<IEnumerable<ProductDto>> SearchProductsByNameAsync(string searchTerm);
    Task<ProductDto> CreateProductAsync(CreateProductDto productDto);
    Task<ProductDto> UpdateProductAsync(string id, UpdateProductDto productDto);
    Task<bool> DeleteProductAsync(string id);
}
