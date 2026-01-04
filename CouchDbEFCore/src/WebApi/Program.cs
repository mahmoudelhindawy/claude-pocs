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
        Title = "CouchDB API with Entity Framework Core",
        Version = "v1",
        Description = "Clean Architecture API using Entity Framework Core patterns with CouchDB backend",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Mahmoud El Hindawy",
            Email = "mahmoud.elhendawy@gmail.com"
        }
    });
});

// Configure CouchDB Settings
builder.Services.Configure<CouchDbSettings>(
    builder.Configuration.GetSection("CouchDbSettings"));

// Register Entity Framework Core DbContext
builder.Services.AddDbContext<CouchDbContext>(options =>
{
    // Using InMemory as cache layer - actual data stored in CouchDB
    options.UseInMemoryDatabase("CouchDbCache");
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

var app = builder.Build();

// Ensure CouchDB database exists
using (var scope = app.Services.CreateScope())
{
    try
    {
        var couchDbContext = scope.ServiceProvider.GetRequiredService<CouchDbContext>();
        await couchDbContext.EnsureDatabaseExistsAsync();
        app.Logger.LogInformation("CouchDB database initialized successfully");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Error initializing CouchDB database");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "CouchDB API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Logger.LogInformation("Application started successfully");
app.Logger.LogInformation("Swagger UI available at: /swagger");

app.Run();
