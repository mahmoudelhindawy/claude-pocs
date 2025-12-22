using Microsoft.Extensions.Options;
using MyCouch;

namespace Infrastructure.Persistence;

public class CouchDbContext
{
    private readonly CouchDbSettings _settings;
    private MyCouchClient? _client;

    public CouchDbContext(IOptions<CouchDbSettings> settings)
    {
        _settings = settings.Value;
    }

    public MyCouchClient GetClient()
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
