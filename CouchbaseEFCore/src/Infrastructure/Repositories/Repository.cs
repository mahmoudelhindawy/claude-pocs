using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Generic Repository using standard Entity Framework Core with Couchbase provider
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
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<T> CreateAsync(T entity)
    {
        if (string.IsNullOrEmpty(entity.Id))
        {
            entity.Id = Guid.NewGuid().ToString();
        }
        
        entity.CreatedAt = DateTime.UtcNow;
        
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        
        return entity;
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        
        return entity;
    }

    public virtual async Task<bool> DeleteAsync(string id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity == null)
        {
            return false;
        }
        
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
        
        return true;
    }

    public virtual async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
