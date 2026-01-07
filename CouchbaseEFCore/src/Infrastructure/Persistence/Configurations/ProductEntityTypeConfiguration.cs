using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Couchbase.EntityFrameworkCore.Extensions;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Type Configuration for Product entity with nested complex types
/// Demonstrates how to configure nested objects for Couchbase document storage
/// </summary>
public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
    private readonly CouchbaseContext _context;

    public ProductEntityTypeConfiguration(CouchbaseContext context)
    {
        _context = context;
    }

    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Map to Couchbase collection
        builder.ToCouchbaseCollection(_context, "products");

        // Primary Key
        builder.HasKey(e => e.Id);

        // Simple Property Configurations
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

        // Ignore CAS (managed by Couchbase)
        builder.Ignore(e => e.Cas);

        // ====================================================================
        // NESTED OBJECT CONFIGURATION
        // ====================================================================

        // Configure Warehouse as an owned entity (nested object in JSON)
        builder.OwnsOne(p => p.Warehouse, warehouse =>
        {
            warehouse.Property(w => w.Name)
                .HasColumnName("warehouse_name")
                .HasMaxLength(100);

            warehouse.Property(w => w.Code)
                .HasColumnName("warehouse_code")
                .HasMaxLength(20);

            warehouse.Property(w => w.ContactPerson)
                .HasColumnName("warehouse_contactPerson")
                .HasMaxLength(100);

            warehouse.Property(w => w.ContactPhone)
                .HasColumnName("warehouse_contactPhone")
                .HasMaxLength(20);

            // Configure nested Address object (2nd level nesting)
            warehouse.OwnsOne(w => w.Address, address =>
            {
                address.Property(a => a.Street)
                    .HasColumnName("warehouse_address_street")
                    .HasMaxLength(200);

                address.Property(a => a.City)
                    .HasColumnName("warehouse_address_city")
                    .HasMaxLength(100);

                address.Property(a => a.State)
                    .HasColumnName("warehouse_address_state")
                    .HasMaxLength(50);

                address.Property(a => a.ZipCode)
                    .HasColumnName("warehouse_address_zipCode")
                    .HasMaxLength(20);

                address.Property(a => a.Country)
                    .HasColumnName("warehouse_address_country")
                    .HasMaxLength(100);

                // Configure GeoCoordinates (3rd level nesting)
                address.OwnsOne(a => a.Coordinates, coords =>
                {
                    coords.Property(c => c.Latitude)
                        .HasColumnName("warehouse_address_coordinates_latitude");

                    coords.Property(c => c.Longitude)
                        .HasColumnName("warehouse_address_coordinates_longitude");
                });
            });
        });

        // Configure Specifications as a collection of owned entities
        builder.OwnsMany(p => p.Specifications, spec =>
        {
            // Each specification will be stored as an object in an array
            spec.Property(s => s.Key)
                .HasColumnName("key")
                .IsRequired()
                .HasMaxLength(100);

            spec.Property(s => s.Value)
                .HasColumnName("value")
                .IsRequired()
                .HasMaxLength(500);

            spec.Property(s => s.Unit)
                .HasColumnName("unit")
                .HasMaxLength(50);
        });

        // Indexes
        builder.HasIndex(e => e.Category)
            .HasDatabaseName("idx_category");

        builder.HasIndex(e => e.IsActive)
            .HasDatabaseName("idx_isActive");

        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("idx_createdAt");
    }
}
