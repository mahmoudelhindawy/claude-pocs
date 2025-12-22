using Application.Interfaces;
using Application.Services;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CouchDB
builder.Services.Configure<CouchDbSettings>(
    builder.Configuration.GetSection("CouchDbSettings"));

// Register CouchDB Context
builder.Services.AddSingleton<CouchDbContext>();

// Register Repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Register Services
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

// Ensure CouchDB database exists
using (var scope = app.Services.CreateScope())
{
    var couchDbContext = scope.ServiceProvider.GetRequiredService<CouchDbContext>();
    await couchDbContext.EnsureDatabaseExistsAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
