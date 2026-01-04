using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Product : BaseEntity
{
    public Product()
    {
        Type = "product"; // Document type discriminator for Couchbase
    }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public decimal Price { get; set; }
    
    [Required]
    public int Quantity { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
}
