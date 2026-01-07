namespace Application.DTOs;

/// <summary>
/// DTO for Product with nested objects
/// </summary>
public class ProductDto
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = "Product";
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Nested objects
    public WarehouseLocationDto? Warehouse { get; set; }
    public List<ProductSpecificationDto> Specifications { get; set; } = new();
}

/// <summary>
/// DTO for creating a product
/// </summary>
public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    
    // Optional nested objects
    public WarehouseLocationDto? Warehouse { get; set; }
    public List<ProductSpecificationDto>? Specifications { get; set; }
}

/// <summary>
/// DTO for updating a product
/// </summary>
public class UpdateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    
    // Optional nested objects
    public WarehouseLocationDto? Warehouse { get; set; }
    public List<ProductSpecificationDto>? Specifications { get; set; }
}

/// <summary>
/// DTO for Warehouse Location
/// </summary>
public class WarehouseLocationDto
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public AddressDto Address { get; set; } = new();
    public string ContactPerson { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
}

/// <summary>
/// DTO for Address
/// </summary>
public class AddressDto
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public GeoCoordinatesDto? Coordinates { get; set; }
}

/// <summary>
/// DTO for Geographic Coordinates
/// </summary>
public class GeoCoordinatesDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

/// <summary>
/// DTO for Product Specification
/// </summary>
public class ProductSpecificationDto
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
}
