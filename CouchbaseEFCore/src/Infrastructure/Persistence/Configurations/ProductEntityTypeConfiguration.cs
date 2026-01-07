using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Type Configuration for Product entity
/// Receives DbContext via constructor for advanced configuration scenarios
/// </summary>
public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
    private readonly CouchbaseContext _context;

    /// <summary>
    /// Constructor that receives the DbContext instance
    /// This allows access to DbContext properties, settings, and methods during configuration
    /// </summary>
    public ProductEntityTypeConfiguration(CouchbaseContext context)
    {
        _context = context;
    }

    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // You can now access DbContext properties
        // Example: var providerName = _context.Database.ProviderName;
        // Example: var connectionString = _context.Database.GetConnectionString();

        // Table/Collection name
        builder.ToTable("products");

        // Primary Key
        builder.HasKey(e => e.Id);

        // Property Configurations
        builder.Property(e => e.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(e => e.Type)
            .HasColumnName("type")
            .HasMaxLength(50);

        builder.Property(e => e.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasColumnName("description")
            .HasMaxLength(1000);

        builder.Property(e => e.Category)
            .HasColumnName("category")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Price)
            .HasColumnName("price")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(e => e.Quantity)
            .HasColumnName("quantity")
            .IsRequired();

        builder.Property(e => e.IsActive)
            .HasColumnName("isActive")
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .HasColumnName("createdAt")
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .HasColumnName("updatedAt");

        // Ignore properties that shouldn't be persisted
        builder.Ignore(e => e.Cas);

        // Indexes (if supported by Couchbase EF Core provider)
        builder.HasIndex(e => e.Category)
            .HasDatabaseName("idx_category");

        builder.HasIndex(e => e.IsActive)
            .HasDatabaseName("idx_isActive");

        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("idx_createdAt");
    }
}
