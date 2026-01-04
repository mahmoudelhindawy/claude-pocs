using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MyCouch.Requests;
using Newtonsoft.Json;

namespace Infrastructure.Repositories;

/// <summary>
/// Generic Repository implementation using Entity Framework Core patterns with CouchDB
/// This bridges EF Core patterns with CouchDB operations
/// </summary>
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly CouchDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(CouchDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(string id)
    {
        try
        {
            var client = _context.GetCouchClient();
            var response = await client.Documents.GetAsync(id);
            
            if (!response.IsSuccess)
            {
                return null;
            }

            var entity = JsonConvert.DeserializeObject<T>(response.Content);
            if (entity != null)
            {
                entity.Id = response.Id;
                entity.Rev = response.Rev;
            }
            
            return entity;
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
            var client = _context.GetCouchClient();
            var query = new QueryViewRequest("_all_docs")
            {
                IncludeDocs = true
            };

            var response = await client.Views.QueryAsync<string, T>(query);
            return response.Rows
                .Where(r => r.IncludedDoc != null)
                .Select(r => r.IncludedDoc)
                .ToList()!;
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
            
            var client = _context.GetCouchClient();
            var json = JsonConvert.SerializeObject(entity);
            var response = await client.Documents.PostAsync(json);

            if (!response.IsSuccess)
            {
                throw new Exception($"Failed to create document: {response.Reason}");
            }

            entity.Id = response.Id;
            entity.Rev = response.Rev;
            
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
            
            var client = _context.GetCouchClient();
            
            // Serialize entity to dictionary to preserve CouchDB fields
            var json = JsonConvert.SerializeObject(entity);
            var doc = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            
            if (doc == null)
            {
                throw new Exception("Failed to serialize entity");
            }
            
            doc["_id"] = entity.Id;
            if (!string.IsNullOrEmpty(entity.Rev))
            {
                doc["_rev"] = entity.Rev;
            }

            var docJson = JsonConvert.SerializeObject(doc);
            var response = await client.Documents.PutAsync(entity.Id, entity.Rev, docJson);

            if (!response.IsSuccess)
            {
                throw new Exception($"Failed to update document: {response.Reason}");
            }

            entity.Rev = response.Rev;
            
            // Update in EF Core DbSet for consistency
            _dbSet.Update(entity);
            
            return entity;
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
            var client = _context.GetCouchClient();
            
            // Get the document first to get its revision
            var getResponse = await client.Documents.GetAsync(id);
            if (!getResponse.IsSuccess)
            {
                return false;
            }

            // Delete the document
            var deleteResponse = await client.Documents.DeleteAsync(id, getResponse.Rev);
            
            if (deleteResponse.IsSuccess)
            {
                // Remove from EF Core DbSet for consistency
                var entity = await _dbSet.FindAsync(id);
                if (entity != null)
                {
                    _dbSet.Remove(entity);
                }
            }
            
            return deleteResponse.IsSuccess;
        }
        catch
        {
            return false;
        }
    }

    public virtual async Task<int> SaveChangesAsync()
    {
        // In this hybrid approach, changes are saved directly to CouchDB
        // This method is here for EF Core compatibility but actual saves happen in Create/Update/Delete
        return await _context.SaveChangesAsync();
    }
}
