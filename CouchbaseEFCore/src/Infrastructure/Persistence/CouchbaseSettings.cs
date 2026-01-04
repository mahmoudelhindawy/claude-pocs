namespace Infrastructure.Persistence;

public class CouchbaseSettings
{
    public string ConnectionString { get; set; } = "couchbase://localhost";
    public string Username { get; set; } = "Administrator";
    public string Password { get; set; } = "password";
    public string BucketName { get; set; } = "products";
    public string ScopeName { get; set; } = "_default";
    public string CollectionName { get; set; } = "_default";
}
