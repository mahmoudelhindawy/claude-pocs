using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Type Configuration for Product entity
/// Isolates EF Core mapping configuration from the Domain layer
/// </summary>
public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Table/Collection name (Couchbase uses collections)
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
