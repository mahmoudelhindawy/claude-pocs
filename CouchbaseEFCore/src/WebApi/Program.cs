using Application.Interfaces;
using Application.Services;
using Couchbase.Extensions.DependencyInjection;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Couchbase API with Entity Framework Core",
        Version = "v1",
        Description = "Clean Architecture API using Entity Framework Core with Couchbase as the database provider",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Mahmoud El Hindawy",
            Email = "mahmoud.elhendawy@gmail.com"
        }
    });
});

// Configure Couchbase Settings
var couchbaseSettings = builder.Configuration.GetSection("CouchbaseSettings").Get<CouchbaseSettings>();
builder.Services.Configure<CouchbaseSettings>(builder.Configuration.GetSection("CouchbaseSettings"));

// Register Couchbase cluster and bucket providers
builder.Services.AddCouchbase(options =>
{
    options.ConnectionString = couchbaseSettings!.ConnectionString;
    options.UserName = couchbaseSettings.Username;
    options.Password = couchbaseSettings.Password;
});

// Register named bucket provider
builder.Services.AddCouchbaseBucket<INamedBucketProvider>(couchbaseSettings!.BucketName);

// Register Entity Framework Core DbContext with InMemory database
// Note: We use InMemory for EF Core compatibility, actual data is stored in Couchbase via repositories
builder.Services.AddDbContext<CouchbaseContext>((serviceProvider, options) =>
{
    options.UseInMemoryDatabase("CouchbaseApp");
    options.EnableSensitiveDataLogging();
});

// Register Database Creator
builder.Services.AddScoped<ICouchbaseDatabaseCreator, CouchbaseDatabaseCreator>();

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

// Ensure Couchbase bucket exists
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbCreator = scope.ServiceProvider.GetRequiredService<ICouchbaseDatabaseCreator>();
        await dbCreator.EnsureBucketCreatedAsync();
        app.Logger.LogInformation("Couchbase bucket initialized successfully");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Error initializing Couchbase bucket");
        app.Logger.LogWarning("Application will continue, but database operations may fail");
    }
}

// Configure the HTTP request pipeline.
// Enable Swagger in all environments (Development and Production)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Couchbase API v1");
    options.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root (http://localhost:5000)
    options.DocumentTitle = "Couchbase EF Core API";
});

// Comment out HTTPS redirection to avoid the warning
// app.UseHttpsRedirection();

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Logger.LogInformation("Application started successfully");
app.Logger.LogInformation("Swagger UI available at: http://localhost:5000");
app.Logger.LogInformation("Health check available at: http://localhost:5000/health");
app.Logger.LogInformation("Using Couchbase as EF Core database provider");

app.Run();
