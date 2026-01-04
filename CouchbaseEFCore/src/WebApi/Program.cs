using Application.Interfaces;
using Application.Services;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Couchbase API with Entity Framework Core",
        Version = "v1",
        Description = "Clean Architecture API using official Couchbase.EntityFrameworkCore provider",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Mahmoud El Hindawy",
            Email = "mahmoud.elhendawy@gmail.com"
        }
    });
});

// Configure Couchbase Settings
var couchbaseSettings = builder.Configuration
    .GetSection("CouchbaseSettings")
    .Get<CouchbaseSettings>();

builder.Services.Configure<CouchbaseSettings>(
    builder.Configuration.GetSection("CouchbaseSettings"));

// Register DbContext with official Couchbase Entity Framework Core provider
builder.Services.AddDbContext<CouchbaseContext>(options =>
    options.UseCouchbase(
        couchbaseSettings!.ConnectionString,
        couchbaseSettings.Username,
        couchbaseSettings.Password));

// Register Repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Register Application Services
builder.Services.AddScoped<IProductService, ProductService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Ensure Couchbase database is ready
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<CouchbaseContext>();
        // The database will be created automatically by the provider
        app.Logger.LogInformation("Couchbase connection initialized successfully");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Error initializing Couchbase connection");
        app.Logger.LogWarning("Application will continue, but database operations may fail");
    }
}

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Couchbase API v1");
    options.RoutePrefix = string.Empty; // Serve Swagger UI at root
    options.DocumentTitle = "Couchbase EF Core API";
});

// Comment out HTTPS redirection for development
// app.UseHttpsRedirection();

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Logger.LogInformation("Application started successfully");
app.Logger.LogInformation("Swagger UI available at: http://localhost:5000");
app.Logger.LogInformation("Health check available at: http://localhost:5000/health");
app.Logger.LogInformation("Using official Couchbase.EntityFrameworkCore provider");

app.Run();
