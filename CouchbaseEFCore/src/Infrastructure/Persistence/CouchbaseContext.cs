using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.Persistence;

/// <summary>
/// DbContext for Couchbase using the official Couchbase.EntityFrameworkCore provider
/// Automatically discovers and applies IEntityTypeConfiguration classes
/// Passes 'this' (DbContext) to configuration constructors for advanced scenarios
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

        // Apply all IEntityTypeConfiguration implementations
        // Pass 'this' (DbContext) to each configuration constructor
        var configurationTypes = Assembly.GetExecutingAssembly().DefinedTypes
            .Where(t => !t.IsAbstract && 
                        !t.IsInterface && 
                        t.ImplementedInterfaces.Any(i => 
                            i.IsGenericType && 
                            i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)));

        foreach (var configurationType in configurationTypes)
        {
            object? configurationInstance = null;

            try
            {
                // Try to create instance with DbContext parameter
                configurationInstance = Activator.CreateInstance(configurationType, this);
            }
            catch (MissingMethodException)
            {
                try
                {
                    // Fallback: Try parameterless constructor
                    configurationInstance = Activator.CreateInstance(configurationType);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Unable to create instance of {configurationType.Name}. " +
                        $"Ensure it has either a constructor accepting {nameof(CouchbaseContext)} " +
                        $"or a parameterless constructor.", ex);
                }
            }

            if (configurationInstance != null)
            {
                modelBuilder.ApplyConfiguration((dynamic)configurationInstance);
            }
        }
    }
}
