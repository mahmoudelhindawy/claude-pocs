using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto?> GetProductByIdAsync(string id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return product == null ? null : MapToDto(product);
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(string category)
    {
        var products = await _productRepository.GetByCategoryAsync(category);
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> GetActiveProductsAsync()
    {
        var products = await _productRepository.GetActiveProductsAsync();
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsByNameAsync(string searchTerm)
    {
        var products = await _productRepository.SearchByNameAsync(searchTerm);
        return products.Select(MapToDto);
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto productDto)
    {
        var product = new Product
        {
            Id = Guid.NewGuid().ToString(),
            Name = productDto.Name,
            Description = productDto.Description,
            Price = productDto.Price,
            Quantity = productDto.Quantity,
            Category = productDto.Category,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var createdProduct = await _productRepository.CreateAsync(product);
        await _productRepository.SaveChangesAsync();
        
        return MapToDto(createdProduct);
    }

    public async Task<ProductDto> UpdateProductAsync(string id, UpdateProductDto productDto)
    {
        var existingProduct = await _productRepository.GetByIdAsync(id);
        if (existingProduct == null)
        {
            throw new KeyNotFoundException($"Product with ID {id} not found");
        }

        existingProduct.Name = productDto.Name;
        existingProduct.Description = productDto.Description;
        existingProduct.Price = productDto.Price;
        existingProduct.Quantity = productDto.Quantity;
        existingProduct.Category = productDto.Category;
        existingProduct.IsActive = productDto.IsActive;
        existingProduct.UpdatedAt = DateTime.UtcNow;

        var updatedProduct = await _productRepository.UpdateAsync(existingProduct);
        await _productRepository.SaveChangesAsync();
        
        return MapToDto(updatedProduct);
    }

    public async Task<bool> DeleteProductAsync(string id)
    {
        var result = await _productRepository.DeleteAsync(id);
        if (result)
        {
            await _productRepository.SaveChangesAsync();
        }
        return result;
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity,
            Category = product.Category,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}
