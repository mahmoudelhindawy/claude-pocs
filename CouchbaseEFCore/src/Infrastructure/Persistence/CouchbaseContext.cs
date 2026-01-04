using Couchbase;
using Couchbase.KeyValue;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Infrastructure.Persistence;

/// <summary>
/// DbContext for Couchbase using Entity Framework Core patterns
/// This is a hybrid approach that uses EF Core patterns with Couchbase backend
/// </summary>
public class CouchbaseContext : DbContext
{
    private readonly CouchbaseSettings _settings;
    private ICluster? _cluster;
    private IBucket? _bucket;
    private IScope? _scope;
    private ICouchbaseCollection? _collection;

    public CouchbaseContext(DbContextOptions<CouchbaseContext> options, IOptions<CouchbaseSettings> settings) 
        : base(options)
    {
        _settings = settings.Value;
    }

    public DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Use InMemory provider as EF Core doesn't have native Couchbase provider
        // We'll handle Couchbase operations in the repository layer
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseInMemoryDatabase("CouchbaseCache");
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
            entity.Ignore(e => e.Cas); // CAS is not persisted, it's runtime only
        });
    }

    /// <summary>
    /// Get Couchbase cluster instance
    /// </summary>
    public async Task<ICluster> GetClusterAsync()
    {
        if (_cluster == null)
        {
            var options = new ClusterOptions
            {
                UserName = _settings.Username,
                Password = _settings.Password
            };

            _cluster = await Cluster.ConnectAsync(_settings.ConnectionString, options);
        }
        return _cluster;
    }

    /// <summary>
    /// Get Couchbase bucket instance
    /// </summary>
    public async Task<IBucket> GetBucketAsync()
    {
        if (_bucket == null)
        {
            var cluster = await GetClusterAsync();
            _bucket = await cluster.BucketAsync(_settings.BucketName);
        }
        return _bucket;
    }

    /// <summary>
    /// Get Couchbase scope instance
    /// </summary>
    public async Task<IScope> GetScopeAsync()
    {
        if (_scope == null)
        {
            var bucket = await GetBucketAsync();
            _scope = bucket.Scope(_settings.ScopeName);
        }
        return _scope;
    }

    /// <summary>
    /// Get Couchbase collection instance
    /// </summary>
    public async Task<ICouchbaseCollection> GetCollectionAsync()
    {
        if (_collection == null)
        {
            var scope = await GetScopeAsync();
            _collection = scope.Collection(_settings.CollectionName);
        }
        return _collection;
    }

    /// <summary>
    /// Ensure Couchbase bucket and indexes exist
    /// </summary>
    public async Task EnsureBucketExistsAsync()
    {
        try
        {
            var cluster = await GetClusterAsync();
            var bucketManager = cluster.Buckets;

            // Check if bucket exists
            try
            {
                await bucketManager.GetBucketAsync(_settings.BucketName);
            }
            catch
            {
                // Create bucket if it doesn't exist
                var bucketSettings = new Couchbase.Management.Buckets.BucketSettings
                {
                    Name = _settings.BucketName,
                    BucketType = Couchbase.Management.Buckets.BucketType.Couchbase,
                    RamQuotaMB = 256
                };
                await bucketManager.CreateBucketAsync(bucketSettings);
                
                // Wait for bucket to be ready
                await Task.Delay(3000);
            }

            // Create primary index for queries
            await CreatePrimaryIndexAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to ensure bucket exists: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Create primary index for N1QL queries
    /// </summary>
    private async Task CreatePrimaryIndexAsync()
    {
        try
        {
            var cluster = await GetClusterAsync();
            var query = $"CREATE PRIMARY INDEX ON `{_settings.BucketName}` IF NOT EXISTS";
            await cluster.QueryAsync<dynamic>(query);
        }
        catch
        {
            // Index might already exist, ignore
        }
    }

    /// <summary>
    /// Dispose Couchbase resources
    /// </summary>
    public override void Dispose()
    {
        _cluster?.Dispose();
        base.Dispose();
    }

    /// <summary>
    /// Dispose Couchbase resources asynchronously
    /// </summary>
    public override async ValueTask DisposeAsync()
    {
        if (_cluster != null)
        {
            await _cluster.DisposeAsync();
        }
        await base.DisposeAsync();
    }
}
