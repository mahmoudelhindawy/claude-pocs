using Couchbase.Extensions.DependencyInjection;
using Couchbase.Management.Buckets;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

namespace Infrastructure.Persistence;

/// <summary>
/// Couchbase database creator implementation
/// </summary>
public class CouchbaseDatabaseCreator : ICouchbaseDatabaseCreator
{
    private readonly IClusterProvider _clusterProvider;
    private readonly CouchbaseSettings _settings;

    public CouchbaseDatabaseCreator(
        IClusterProvider clusterProvider,
        IOptions<CouchbaseSettings> settings)
    {
        _clusterProvider = clusterProvider;
        _settings = settings.Value;
    }

    public async Task<bool> EnsureCreatedAsync(CancellationToken cancellationToken = default)
    {
        await EnsureBucketCreatedAsync(cancellationToken);
        return true;
    }

    public bool EnsureCreated()
    {
        return EnsureCreatedAsync().GetAwaiter().GetResult();
    }

    public async Task<bool> EnsureDeletedAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var cluster = await _clusterProvider.GetClusterAsync();
            var bucketManager = cluster.Buckets;
            await bucketManager.DropBucketAsync(_settings.BucketName);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool EnsureDeleted()
    {
        return EnsureDeletedAsync().GetAwaiter().GetResult();
    }

    public async Task EnsureBucketCreatedAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var cluster = await _clusterProvider.GetClusterAsync();
            var bucketManager = cluster.Buckets;

            // Check if bucket exists
            try
            {
                await bucketManager.GetBucketAsync(_settings.BucketName);
            }
            catch
            {
                // Create bucket if it doesn't exist
                var bucketSettings = new BucketSettings
                {
                    Name = _settings.BucketName,
                    BucketType = BucketType.Couchbase,
                    RamQuotaMB = 256
                };
                
                await bucketManager.CreateBucketAsync(bucketSettings);
                
                // Wait for bucket to be ready
                await Task.Delay(3000, cancellationToken);
            }

            // Create primary index
            await CreatePrimaryIndexAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to ensure bucket exists: {ex.Message}", ex);
        }
    }

    private async Task CreatePrimaryIndexAsync(CancellationToken cancellationToken)
    {
        try
        {
            var cluster = await _clusterProvider.GetClusterAsync();
            var query = $"CREATE PRIMARY INDEX ON `{_settings.BucketName}` IF NOT EXISTS";
            await cluster.QueryAsync<dynamic>(query);
        }
        catch
        {
            // Index might already exist
        }
    }

    public bool CanConnect()
    {
        return CanConnectAsync().GetAwaiter().GetResult();
    }

    public async Task<bool> CanConnectAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var cluster = await _clusterProvider.GetClusterAsync();
            await cluster.QueryAsync<dynamic>("SELECT 1");
            return true;
        }
        catch
        {
            return false;
        }
    }
}
