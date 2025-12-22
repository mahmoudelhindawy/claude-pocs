using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using MyCouch;
using MyCouch.Requests;
using Newtonsoft.Json;

namespace Infrastructure.Repositories;

public class CouchDbRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly CouchDbContext _context;
    protected readonly MyCouchClient _client;

    public CouchDbRepository(CouchDbContext context)
    {
        _context = context;
        _client = _context.GetClient();
    }

    public async Task<T?> GetByIdAsync(string id)
    {
        try
        {
            var response = await _client.Documents.GetAsync(id);
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

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var query = new QueryViewRequest("_all_docs")
        {
            IncludeDocs = true
        };

        var response = await _client.Views.QueryAsync<string, T>(query);
        return response.Rows.Select(r => r.IncludedDoc).Where(doc => doc != null)!;
    }

    public async Task<T> CreateAsync(T entity)
    {
        entity.Id = Guid.NewGuid().ToString();
        entity.CreatedAt = DateTime.UtcNow;

        var json = JsonConvert.SerializeObject(entity);
        var response = await _client.Documents.PostAsync(json);

        if (!response.IsSuccess)
        {
            throw new Exception($"Failed to create document: {response.Reason}");
        }

        entity.Id = response.Id;
        entity.Rev = response.Rev;
        return entity;
    }

    public async Task<T> UpdateAsync(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;

        var json = JsonConvert.SerializeObject(new
        {
            _id = entity.Id,
            _rev = entity.Rev,
            entity.CreatedAt,
            entity.UpdatedAt,
            // Add other properties dynamically
        });

        // Serialize the full entity
        var fullJson = JsonConvert.SerializeObject(entity);
        var doc = JsonConvert.DeserializeObject<Dictionary<string, object>>(fullJson);
        doc!["_id"] = entity.Id;
        doc["_rev"] = entity.Rev!;

        var docJson = JsonConvert.SerializeObject(doc);
        var response = await _client.Documents.PutAsync(entity.Id, entity.Rev, docJson);

        if (!response.IsSuccess)
        {
            throw new Exception($"Failed to update document: {response.Reason}");
        }

        entity.Rev = response.Rev;
        return entity;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        try
        {
            var getResponse = await _client.Documents.GetAsync(id);
            if (!getResponse.IsSuccess)
            {
                return false;
            }

            var deleteResponse = await _client.Documents.DeleteAsync(id, getResponse.Rev);
            return deleteResponse.IsSuccess;
        }
        catch
        {
            return false;
        }
    }
}
