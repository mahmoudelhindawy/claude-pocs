using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Couchbase.EntityFrameworkCore.Extensions;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Type Configuration for Product entity
/// Uses ToCouchbaseCollection to properly map to Couchbase collection
/// Requires DbContext via constructor for Couchbase-specific collection mapping
/// </summary>
public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
    private readonly CouchbaseContext _context;

    /// <summary>
    /// Constructor that receives the DbContext instance
    /// Required for ToCouchbaseCollection extension method
    /// </summary>
    public ProductEntityTypeConfiguration(CouchbaseContext context)
    {
        _context = context;
    }

    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Use Couchbase-specific collection mapping
        // This is required instead of builder.ToTable() for Couchbase
        builder.ToCouchbaseCollection(_context, "products");

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
        // CAS is managed by Couchbase automatically
        builder.Ignore(e => e.Cas);

        // Indexes for Couchbase
        builder.HasIndex(e => e.Category)
            .HasDatabaseName("idx_category");

        builder.HasIndex(e => e.IsActive)
            .HasDatabaseName("idx_isActive");

        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("idx_createdAt");
    }
}
