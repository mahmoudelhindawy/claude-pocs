using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Infrastructure.Persistence;

/// <summary>
/// Extension methods for configuring Couchbase with Entity Framework Core
/// </summary>
public static class CouchbaseDbContextOptionsExtensions
{
    /// <summary>
    /// Configures the context to connect to a Couchbase database
    /// </summary>
    public static DbContextOptionsBuilder UseCouchbase(
        this DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder == null)
        {
            throw new ArgumentNullException(nameof(optionsBuilder));
        }

        // Register Couchbase-specific services
        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder)
            .AddOrUpdateExtension(new CouchbaseOptionsExtension());

        return optionsBuilder;
    }

    /// <summary>
    /// Configures the context to connect to a Couchbase database with options
    /// </summary>
    public static DbContextOptionsBuilder<TContext> UseCouchbase<TContext>(
        this DbContextOptionsBuilder<TContext> optionsBuilder)
        where TContext : DbContext
    {
        return (DbContextOptionsBuilder<TContext>)UseCouchbase(
            (DbContextOptionsBuilder)optionsBuilder);
    }
}
