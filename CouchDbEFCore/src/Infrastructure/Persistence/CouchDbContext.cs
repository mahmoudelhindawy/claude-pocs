using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyCouch;

namespace Infrastructure.Persistence;

/// <summary>
/// DbContext for CouchDB using Entity Framework Core patterns
/// This is a hybrid approach that uses EF Core patterns with CouchDB backend
/// </summary>
public class CouchDbContext : DbContext
{
    private readonly CouchDbSettings _settings;
    private MyCouchClient? _client;

    public CouchDbContext(DbContextOptions<CouchDbContext> options, IOptions<CouchDbSettings> settings) 
        : base(options)
    {
        _settings = settings.Value;
    }

    public DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Use InMemory provider as EF Core doesn't have native CouchDB provider
        // We'll handle CouchDB operations in the repository layer
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseInMemoryDatabase("CouchDbCache");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Product entity
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("_id");
            entity.Property(e => e.Rev).HasColumnName("_rev");
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Price).HasPrecision(18, 2);
        });
    }

    /// <summary>
    /// Get CouchDB client for direct database operations
    /// </summary>
    public MyCouchClient GetCouchClient()
    {
        if (_client == null)
        {
            var connectionInfo = new DbConnectionInfo(_settings.Url, _settings.DatabaseName)
            {
                BasicAuth = new BasicAuthString(_settings.Username, _settings.Password)
            };
            _client = new MyCouchClient(connectionInfo);
        }
        return _client;
    }

    /// <summary>
    /// Ensure CouchDB database exists
    /// </summary>
    public async Task EnsureDatabaseExistsAsync()
    {
        try
        {
            var serverClient = new MyCouchServerClient(_settings.Url, new ServerConnectionInfo
            {
                BasicAuth = new BasicAuthString(_settings.Username, _settings.Password)
            });

            var dbsResponse = await serverClient.Databases.GetAsync();
            if (!dbsResponse.Value.Contains(_settings.DatabaseName))
            {
                await serverClient.Databases.PutAsync(_settings.DatabaseName);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to ensure database exists: {ex.Message}", ex);
        }
    }
}
