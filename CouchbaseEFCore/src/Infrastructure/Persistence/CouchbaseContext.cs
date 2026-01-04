using Couchbase.EntityFrameworkCore.Extensions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

/// <summary>
/// DbContext for Couchbase using the official Couchbase.EntityFrameworkCore provider
/// Documentation: https://docs.couchbase.com/efcore-provider/current/entity-framework-core-configuration.html
/// </summary>
public class CouchbaseContext : DbContext
{
    public CouchbaseContext(DbContextOptions<CouchbaseContext> options) 
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Product entity for Couchbase
        modelBuilder.Entity<Product>(entity =>
        {
            // Specify the Couchbase bucket for this entity
            entity.ToCouchbaseBucket("products");
            
            // Configure primary key
            entity.HasKey(e => e.Id);
            
            // Property configurations
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Type).HasColumnName("type");
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            
            // Ignore CAS - it's managed automatically by Couchbase
            entity.Ignore(e => e.Cas);
        });
    }
}
