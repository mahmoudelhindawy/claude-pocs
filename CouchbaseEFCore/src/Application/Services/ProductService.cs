using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _repository.GetAllAsync();
        return products.Select(MapToDto);
    }

    public async Task<ProductDto?> GetByIdAsync(string id)
    {
        var product = await _repository.GetByIdAsync(id);
        return product != null ? MapToDto(product) : null;
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto createDto)
    {
        var product = new Product
        {
            Name = createDto.Name,
            Description = createDto.Description,
            Price = createDto.Price,
            Quantity = createDto.Quantity,
            Category = createDto.Category,
            IsActive = createDto.IsActive,
            Warehouse = createDto.Warehouse != null ? MapToWarehouseEntity(createDto.Warehouse) : null,
            Specifications = createDto.Specifications?.Select(MapToSpecificationEntity).ToList() ?? new()
        };

        var created = await _repository.CreateAsync(product);
        return MapToDto(created);
    }

    public async Task<ProductDto?> UpdateAsync(string id, UpdateProductDto updateDto)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null) return null;

        product.Name = updateDto.Name;
        product.Description = updateDto.Description;
        product.Price = updateDto.Price;
        product.Quantity = updateDto.Quantity;
        product.Category = updateDto.Category;
        product.IsActive = updateDto.IsActive;
        product.Warehouse = updateDto.Warehouse != null ? MapToWarehouseEntity(updateDto.Warehouse) : null;
        product.Specifications = updateDto.Specifications?.Select(MapToSpecificationEntity).ToList() ?? new();

        var updated = await _repository.UpdateAsync(product);
        return MapToDto(updated);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<ProductDto>> GetByCategoryAsync(string category)
    {
        var products = await _repository.GetByCategoryAsync(category);
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> GetActiveProductsAsync()
    {
        var products = await _repository.GetActiveProductsAsync();
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> SearchByNameAsync(string searchTerm)
    {
        var products = await _repository.SearchByNameAsync(searchTerm);
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> GetByWarehouseCityAsync(string city)
    {
        var products = await _repository.GetByWarehouseCityAsync(city);
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> GetByWarehouseCodeAsync(string code)
    {
        var products = await _repository.GetByWarehouseCodeAsync(code);
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> GetBySpecificationAsync(string key, string value)
    {
        var products = await _repository.GetBySpecificationAsync(key, value);
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> GetNearbyProductsAsync(double latitude, double longitude, double radiusKm)
    {
        var products = await _repository.GetNearbyProductsAsync(latitude, longitude, radiusKm);
        return products.Select(MapToDto);
    }

    public async Task<ProductDto?> UpdateWarehouseAsync(string id, WarehouseLocationDto warehouseDto)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null) return null;

        product.Warehouse = MapToWarehouseEntity(warehouseDto);
        var updated = await _repository.UpdateAsync(product);
        return MapToDto(updated);
    }

    public async Task<ProductDto?> AddSpecificationAsync(string id, ProductSpecificationDto specificationDto)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null) return null;

        product.Specifications.Add(MapToSpecificationEntity(specificationDto));
        var updated = await _repository.UpdateAsync(product);
        return MapToDto(updated);
    }

    public async Task<ProductDto?> RemoveSpecificationAsync(string id, string key)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null) return null;

        product.Specifications.RemoveAll(s => s.Key == key);
        var updated = await _repository.UpdateAsync(product);
        return MapToDto(updated);
    }

    // Mapping methods
    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Type = product.Type,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity,
            Category = product.Category,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt,
            Warehouse = product.Warehouse != null ? MapToWarehouseDto(product.Warehouse) : null,
            Specifications = product.Specifications.Select(MapToSpecificationDto).ToList()
        };
    }

    private static WarehouseLocationDto MapToWarehouseDto(WarehouseLocation warehouse)
    {
        return new WarehouseLocationDto
        {
            Name = warehouse.Name,
            Code = warehouse.Code,
            ContactPerson = warehouse.ContactPerson,
            ContactPhone = warehouse.ContactPhone,
            Address = new AddressDto
            {
                Street = warehouse.Address.Street,
                City = warehouse.Address.City,
                State = warehouse.Address.State,
                ZipCode = warehouse.Address.ZipCode,
                Country = warehouse.Address.Country,
                Coordinates = warehouse.Address.Coordinates != null
                    ? new GeoCoordinatesDto
                    {
                        Latitude = warehouse.Address.Coordinates.Latitude,
                        Longitude = warehouse.Address.Coordinates.Longitude
                    }
                    : null
            }
        };
    }

    private static ProductSpecificationDto MapToSpecificationDto(ProductSpecification spec)
    {
        return new ProductSpecificationDto
        {
            Key = spec.Key,
            Value = spec.Value,
            Unit = spec.Unit
        };
    }

    private static WarehouseLocation MapToWarehouseEntity(WarehouseLocationDto dto)
    {
        return new WarehouseLocation
        {
            Name = dto.Name,
            Code = dto.Code,
            ContactPerson = dto.ContactPerson,
            ContactPhone = dto.ContactPhone,
            Address = new Address
            {
                Street = dto.Address.Street,
                City = dto.Address.City,
                State = dto.Address.State,
                ZipCode = dto.Address.ZipCode,
                Country = dto.Address.Country,
                Coordinates = dto.Address.Coordinates != null
                    ? new GeoCoordinates
                    {
                        Latitude = dto.Address.Coordinates.Latitude,
                        Longitude = dto.Address.Coordinates.Longitude
                    }
                    : null
            }
        };
    }

    private static ProductSpecification MapToSpecificationEntity(ProductSpecificationDto dto)
    {
        return new ProductSpecification
        {
            Key = dto.Key,
            Value = dto.Value,
            Unit = dto.Unit
        };
    }
}
