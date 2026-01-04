using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public abstract class BaseEntity
{
    /// <summary>
    /// Unique identifier for the document in Couchbase
    /// </summary>
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Document type for Couchbase queries
    /// </summary>
    public string Type { get; set; } = string.Empty;
    
    /// <summary>
    /// CAS (Compare And Swap) value for optimistic locking in Couchbase
    /// </summary>
    [NotMapped]
    public ulong Cas { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
