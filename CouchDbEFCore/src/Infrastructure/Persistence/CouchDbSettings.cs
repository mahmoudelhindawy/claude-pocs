namespace Infrastructure.Persistence;

public class CouchDbSettings
{
    public string Url { get; set; } = "http://localhost:5984";
    public string DatabaseName { get; set; } = "products_db";
    public string Username { get; set; } = "admin";
    public string Password { get; set; } = "password";
}
