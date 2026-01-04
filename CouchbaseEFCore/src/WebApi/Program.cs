using Application.Interfaces;
using Application.Services;
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
        Description = "Clean Architecture API using Entity Framework Core patterns with Couchbase backend",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Mahmoud El Hindawy",
            Email = "mahmoud.elhendawy@gmail.com"
        }
    });
    
    // Include XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Configure Couchbase Settings
builder.Services.Configure<CouchbaseSettings>(
    builder.Configuration.GetSection("CouchbaseSettings"));

// Register Entity Framework Core DbContext
builder.Services.AddDbContext<CouchbaseContext>(options =>
{
    // Using InMemory as cache layer - actual data stored in Couchbase
    options.UseInMemoryDatabase("CouchbaseCache");
});

// Register Repositories (using EF Core patterns)
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Register Application Services
builder.Services.AddScoped<IProductService, ProductService>();

// Add CORS if needed
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
        var couchbaseContext = scope.ServiceProvider.GetRequiredService<CouchbaseContext>();
        await couchbaseContext.EnsureBucketExistsAsync();
        app.Logger.LogInformation("Couchbase bucket initialized successfully");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Error initializing Couchbase bucket");
        app.Logger.LogWarning("Application will continue, but database operations may fail");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Couchbase API v1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "Couchbase EF Core API";
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Logger.LogInformation("Application started successfully");
app.Logger.LogInformation("Swagger UI available at: /swagger");
app.Logger.LogInformation("Health check available at: /health");

app.Run();
