using Couchbase;
using Couchbase.Extensions.DependencyInjection;
using Couchbase.KeyValue;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

/// <summary>
/// DbContext for Couchbase using Entity Framework Core patterns
/// This uses Couchbase as the actual database provider through dependency injection
/// </summary>
public class CouchbaseContext : DbContext
{
    private readonly IClusterProvider _clusterProvider;
    private readonly IBucketProvider _bucketProvider;

    public CouchbaseContext(
        DbContextOptions<CouchbaseContext> options,
        IClusterProvider clusterProvider,
        IBucketProvider bucketProvider) 
        : base(options)
    {
        _clusterProvider = clusterProvider;
        _bucketProvider = bucketProvider;
    }

    public DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Configure to use Couchbase provider
            optionsBuilder.UseCouchbase();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Product entity
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.Ignore(e => e.Cas); // CAS is runtime-only, not persisted
        });
    }

    /// <summary>
    /// Get Couchbase cluster instance
    /// </summary>
    public async Task<ICluster> GetClusterAsync()
    {
        return await _clusterProvider.GetClusterAsync();
    }

    /// <summary>
    /// Get Couchbase bucket instance
    /// </summary>
    public async Task<IBucket> GetBucketAsync()
    {
        return await _bucketProvider.GetBucketAsync();
    }

    /// <summary>
    /// Get Couchbase collection instance
    /// </summary>
    public async Task<ICouchbaseCollection> GetCollectionAsync()
    {
        var bucket = await GetBucketAsync();
        var scope = bucket.Scope("_default");
        return scope.Collection("_default");
    }
}
