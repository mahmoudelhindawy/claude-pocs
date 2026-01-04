using Couchbase;
using Couchbase.Core.Exceptions;
using Couchbase.Core.Exceptions.KeyValue;
using Couchbase.KeyValue;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Generic Repository implementation using Entity Framework Core patterns with Couchbase
/// This bridges EF Core patterns with Couchbase operations
/// </summary>
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly CouchbaseContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(CouchbaseContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(string id)
    {
        try
        {
            var collection = await _context.GetCollectionAsync();
            var result = await collection.GetAsync(id);
            
            var entity = result.ContentAs<T>();
            if (entity != null)
            {
                entity.Cas = result.Cas; // Store CAS for optimistic locking
            }
            
            return entity;
        }
        catch (DocumentNotFoundException)
        {
            return null;
        }
        catch
        {
            return null;
        }
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        try
        {
            var cluster = await _context.GetClusterAsync();
            
            // Get the type name from the entity
            var typeName = typeof(T).Name.ToLower();
            
            // Use N1QL query to get all documents of this type
            var query = $"SELECT META().id, * FROM `products` WHERE type = '{typeName}'";
            var result = await cluster.QueryAsync<T>(query);
            
            var documents = new List<T>();
            await foreach (var row in result)
            {
                documents.Add(row);
            }
            
            return documents;
        }
        catch
        {
            return Enumerable.Empty<T>();
        }
    }

    public virtual async Task<T> CreateAsync(T entity)
    {
        try
        {
            if (string.IsNullOrEmpty(entity.Id))
            {
                entity.Id = Guid.NewGuid().ToString();
            }
            
            entity.CreatedAt = DateTime.UtcNow;
            
            var collection = await _context.GetCollectionAsync();
            var result = await collection.InsertAsync(entity.Id, entity);
            
            entity.Cas = result.Cas;
            
            // Track in EF Core DbSet for consistency
            _dbSet.Add(entity);
            
            return entity;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error creating entity: {ex.Message}", ex);
        }
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        try
        {
            entity.UpdatedAt = DateTime.UtcNow;
            
            var collection = await _context.GetCollectionAsync();
            
            // Use Replace with CAS for optimistic locking
            var options = new ReplaceOptions();
            if (entity.Cas > 0)
            {
                options.Cas(entity.Cas);
            }
            
            var result = await collection.ReplaceAsync(entity.Id, entity, options);
            entity.Cas = result.Cas;
            
            // Update in EF Core DbSet for consistency
            _dbSet.Update(entity);
            
            return entity;
        }
        catch (CasMismatchException)
        {
            throw new Exception("Document was modified by another process. Please retry.");
        }
        catch (Exception ex)
        {
            throw new Exception($"Error updating entity: {ex.Message}", ex);
        }
    }

    public virtual async Task<bool> DeleteAsync(string id)
    {
        try
        {
            var collection = await _context.GetCollectionAsync();
            await collection.RemoveAsync(id);
            
            // Remove from EF Core DbSet for consistency
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
            
            return true;
        }
        catch (DocumentNotFoundException)
        {
            return false;
        }
        catch
        {
            return false;
        }
    }

    public virtual async Task<int> SaveChangesAsync()
    {
        // In this hybrid approach, changes are saved directly to Couchbase
        // This method is here for EF Core compatibility but actual saves happen in Create/Update/Delete
        return await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Execute N1QL query
    /// </summary>
    protected async Task<IEnumerable<T>> ExecuteQueryAsync(string query)
    {
        var cluster = await _context.GetClusterAsync();
        var result = await cluster.QueryAsync<T>(query);
        
        var documents = new List<T>();
        await foreach (var row in result)
        {
            documents.Add(row);
        }
        
        return documents;
    }
}
