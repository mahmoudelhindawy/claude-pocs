using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Persistence;

/// <summary>
/// Database creator for Couchbase
/// </summary>
public interface ICouchbaseDatabaseCreator : IDatabaseCreator
{
    Task EnsureBucketCreatedAsync(CancellationToken cancellationToken = default);
}
