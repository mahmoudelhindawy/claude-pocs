using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence;

/// <summary>
/// Couchbase-specific options extension for Entity Framework Core
/// </summary>
public class CouchbaseOptionsExtension : IDbContextOptionsExtension
{
    private DbContextOptionsExtensionInfo? _info;

    public DbContextOptionsExtensionInfo Info => _info ??= new CouchbaseOptionsExtensionInfo(this);

    public void ApplyServices(IServiceCollection services)
    {
        // Register Couchbase-specific services
        services.AddScoped<ICouchbaseDatabaseCreator, CouchbaseDatabaseCreator>();
    }

    public void Validate(IDbContextOptions options)
    {
        // Validation logic if needed
    }

    private sealed class CouchbaseOptionsExtensionInfo : DbContextOptionsExtensionInfo
    {
        public CouchbaseOptionsExtensionInfo(IDbContextOptionsExtension extension)
            : base(extension)
        {
        }

        public override bool IsDatabaseProvider => true;

        public override string LogFragment => "using Couchbase";

        public override int GetServiceProviderHashCode() => 0;

        public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
            => other is CouchbaseOptionsExtensionInfo;

        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
        {
            debugInfo["Couchbase"] = "1";
        }
    }
}
