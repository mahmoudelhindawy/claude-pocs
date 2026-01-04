using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public abstract class BaseEntity
{
    [Key]
    [Column("_id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Column("_rev")]
    public string? Rev { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
