namespace Domain.Entities;

/// <summary>
/// Product entity with nested complex type
/// Clean domain entity without infrastructure concerns
/// EF Core configuration is handled in Infrastructure layer via IEntityTypeConfiguration
/// </summary>
public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Nested complex type - Warehouse location information
    /// </summary>
    public WarehouseLocation? Warehouse { get; set; }
    
    /// <summary>
    /// Collection of nested objects - Product specifications
    /// </summary>
    public List<ProductSpecification> Specifications { get; set; } = new();
}

/// <summary>
/// Complex type representing warehouse location
/// This will be stored as a nested object in Couchbase document
/// </summary>
public class WarehouseLocation
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public Address Address { get; set; } = new();
    public string ContactPerson { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
}

/// <summary>
/// Complex type representing an address
/// Demonstrates deeply nested objects
/// </summary>
public class Address
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public GeoCoordinates? Coordinates { get; set; }
}

/// <summary>
/// Geographic coordinates for mapping
/// Demonstrates third-level nesting
/// </summary>
public class GeoCoordinates
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

/// <summary>
/// Product specification (key-value pairs)
/// Demonstrates collection of complex types
/// </summary>
public class ProductSpecification
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
}
