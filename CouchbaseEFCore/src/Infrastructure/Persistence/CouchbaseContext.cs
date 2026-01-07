using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.Persistence;

/// <summary>
/// DbContext for Couchbase using the official Couchbase.EntityFrameworkCore provider
/// Entity configurations are separated into IEntityTypeConfiguration classes
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

        // Apply all IEntityTypeConfiguration implementations from current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Alternative: Apply configurations manually (if you prefer explicit control)
        // modelBuilder.ApplyConfiguration(new ProductEntityTypeConfiguration());
    }
}
