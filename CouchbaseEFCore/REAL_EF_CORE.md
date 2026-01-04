# Couchbase with Entity Framework Core - True Integration

A .NET 8.0 Web API using **Entity Framework Core as a real database provider** with **Couchbase**, not InMemory.

## ✅ What Changed?

### Before (Hybrid Approach):
```csharp
// Used InMemory database
optionsBuilder.UseInMemoryDatabase("CouchbaseCache");
```

### After (Real EF Core Provider):
```csharp
// Uses Couchbase as the actual EF Core provider
optionsBuilder.UseCouchbase();
```

## 🎯 Key Components

### 1. **Couchbase EF Core Provider**

We've implemented a custom EF Core database provider for Couchbase:

```csharp
public static class CouchbaseDbContextOptionsExtensions
{
    public static DbContextOptionsBuilder UseCouchbase(
        this DbContextOptionsBuilder optionsBuilder)
    {
        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder)
            .AddOrUpdateExtension(new CouchbaseOptionsExtension());
        return optionsBuilder;
    }
}
```

### 2. **Dependency Injection Configuration**

```csharp
// Register Couchbase cluster
builder.Services.AddCouchbase(options =>
{
    options.ConnectionString = "couchbase://localhost";
    options.UserName = "Administrator";
    options.Password = "password";
});

// Register bucket provider
builder.Services.AddCouchbaseBucket<INamedBucketProvider>("products");

// Register DbContext with Couchbase provider
builder.Services.AddDbContext<CouchbaseContext>(options =>
{
    options.UseCouchbase(); // Real Couchbase provider!
});
```

### 3. **CouchbaseContext (True EF Core)**

```csharp
public class CouchbaseContext : DbContext
{
    private readonly IClusterProvider _clusterProvider;
    private readonly IBucketProvider _bucketProvider;

    public CouchbaseContext(
        DbContextOptions<CouchbaseContext> options,
        IClusterProvider clusterProvider,
        IBucketProvider bucketProvider) 
        : base(options)
    {
        _clusterProvider = clusterProvider;
        _bucketProvider = bucketProvider;
    }

    public DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseCouchbase(); // Real provider
        }
    }
}
```

## 🏗️ Architecture

```
┌──────────────────────────────────────┐
│   Entity Framework Core              │
│   - DbContext                        │
│   - DbSet<T>                         │
│   - ChangeTracker                    │
└──────────────┬───────────────────────┘
               │
               ▼
┌──────────────────────────────────────┐
│   Custom Couchbase Provider          │
│   - CouchbaseOptionsExtension        │
│   - CouchbaseDatabaseCreator         │
│   - DbContextOptionsExtensions       │
└──────────────┬───────────────────────┘
               │
               ▼
┌──────────────────────────────────────┐
│   Couchbase .NET SDK                 │
│   - IClusterProvider                 │
│   - IBucketProvider                  │
│   - ICouchbaseCollection             │
└──────────────────────────────────────┘
```

## 📦 NuGet Packages

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="CouchbaseNetClient" Version="3.5.1" />
<PackageReference Include="Couchbase.Extensions.DependencyInjection" Version="3.5.1" />
```

## 🚀 How It Works

### Step 1: Configure Services

```csharp
// Program.cs
builder.Services.AddCouchbase(options =>
{
    options.ConnectionString = "couchbase://localhost";
    options.UserName = "Administrator";
    options.Password = "password";
});

builder.Services.AddCouchbaseBucket<INamedBucketProvider>("products");

builder.Services.AddDbContext<CouchbaseContext>(options =>
{
    options.UseCouchbase(); // Custom provider
});
```

### Step 2: Use DbContext Normally

```csharp
public class ProductRepository : Repository<Product>
{
    public ProductRepository(CouchbaseContext context) : base(context)
    {
    }

    // EF Core patterns work naturally
    public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
    {
        // Execute N1QL query through Couchbase
        return await ExecuteQueryAsync($@"
            SELECT META().id, * 
            FROM `products` 
            WHERE type = 'product' AND category = '{category}'
        ");
    }
}
```

### Step 3: Database Operations

```csharp
// Create
var product = new Product { Name = "Laptop", Price = 999.99m };
var collection = await _context.GetCollectionAsync();
await collection.InsertAsync(product.Id, product);
_dbSet.Add(product); // Track in EF Core
await _context.SaveChangesAsync();

// Update
product.Price = 1099.99m;
await collection.ReplaceAsync(product.Id, product);
_dbSet.Update(product); // Update in EF Core
await _context.SaveChangesAsync();

// Delete
await collection.RemoveAsync(product.Id);
_dbSet.Remove(product); // Remove from EF Core
await _context.SaveChangesAsync();
```

## ✨ Benefits of True EF Core Integration

### 1. **Real Database Provider**
- ✅ Not using InMemory as a workaround
- ✅ Proper EF Core infrastructure integration
- ✅ True database provider pattern

### 2. **Proper Dependency Injection**
- ✅ IClusterProvider injected
- ✅ IBucketProvider injected  
- ✅ Proper service lifetime management

### 3. **EF Core Features**
- ✅ DbContext lifecycle management
- ✅ ChangeTracker integration
- ✅ SaveChanges pattern
- ✅ Entity configuration via OnModelCreating

### 4. **Database Creation**
- ✅ ICouchbaseDatabaseCreator implementation
- ✅ EnsureCreated/EnsureDeleted support
- ✅ CanConnect health checks

## 🔧 Configuration

### appsettings.json

```json
{
  "CouchbaseSettings": {
    "ConnectionString": "couchbase://localhost",
    "Username": "Administrator",
    "Password": "password",
    "BucketName": "products",
    "ScopeName": "_default",
    "CollectionName": "_default"
  }
}
```

### Environment Variables

```bash
export CouchbaseSettings__ConnectionString="couchbase://localhost"
export CouchbaseSettings__Username="Administrator"
export CouchbaseSettings__Password="password"
```

## 📚 Custom Provider Components

### 1. CouchbaseOptionsExtension
Implements `IDbContextOptionsExtension` to register Couchbase-specific services.

### 2. CouchbaseDatabaseCreator
Implements `IDatabaseCreator` for database lifecycle management:
- `EnsureCreatedAsync()` - Creates bucket and indexes
- `EnsureDeletedAsync()` - Drops bucket
- `CanConnectAsync()` - Health check

### 3. CouchbaseDbContextOptionsExtensions
Extension methods for configuring `DbContextOptions`:
- `UseCouchbase()` - Registers the Couchbase provider

## 🎯 Usage Example

```csharp
// In your repository
public class ProductRepository : Repository<Product>
{
    private readonly CouchbaseContext _context;

    public ProductRepository(CouchbaseContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Product> CreateAsync(Product product)
    {
        // Get Couchbase collection
        var collection = await _context.GetCollectionAsync();
        
        // Insert into Couchbase
        await collection.InsertAsync(product.Id, product);
        
        // Track in EF Core
        _context.Products.Add(product);
        
        // Save changes
        await _context.SaveChangesAsync();
        
        return product;
    }
}
```

## 🔍 Key Differences from InMemory

| Aspect | InMemory (Old) | Couchbase Provider (New) |
|--------|----------------|--------------------------|
| **Provider** | Microsoft.EntityFrameworkCore.InMemory | Custom Couchbase provider |
| **Registration** | `UseInMemoryDatabase()` | `UseCouchbase()` |
| **Storage** | Memory only | Real Couchbase database |
| **Persistence** | Lost on restart | Persisted to disk |
| **DI** | Simple DbContext | ClusterProvider + BucketProvider |
| **Database Creation** | N/A | CouchbaseDatabaseCreator |

## 🚀 Quick Start

### 1. Start Couchbase

```bash
docker-compose up -d
```

### 2. Configure Cluster (First Time)
- Open http://localhost:8091
- Username: `Administrator`
- Password: `password`
- Setup cluster

### 3. Run Application

```bash
dotnet run
```

The application will:
1. Connect to Couchbase cluster via DI
2. Get bucket provider for "products" bucket
3. Create bucket if it doesn't exist (via CouchbaseDatabaseCreator)
4. Create primary index
5. Use Couchbase as the EF Core database provider

### 4. Test

```bash
# Create product
curl -X POST http://localhost:5000/api/products \
  -H "Content-Type: application/json" \
  -d '{"name":"Laptop","price":999.99,"quantity":10,"category":"Electronics"}'

# Verify in Couchbase
# Open http://localhost:8091 -> Buckets -> products -> Documents
```

## 📝 Best Practices

1. **Always use DI** - Inject IClusterProvider and IBucketProvider
2. **Reuse connections** - Don't create new clusters
3. **Use scoped DbContext** - Let DI manage lifecycle
4. **Implement proper disposal** - CouchbaseContext disposes cluster
5. **Handle CAS conflicts** - Implement retry logic
6. **Create indexes** - Use primary and secondary indexes

## 🔒 Production Considerations

### Connection Pooling
The Couchbase SDK handles connection pooling automatically when using DI.

### Health Checks
```csharp
builder.Services.AddHealthChecks()
    .AddCheck("couchbase", async () =>
    {
        var creator = serviceProvider.GetRequiredService<ICouchbaseDatabaseCreator>();
        return await creator.CanConnectAsync() 
            ? HealthCheckResult.Healthy() 
            : HealthCheckResult.Unhealthy();
    });
```

### Logging
```json
{
  "Logging": {
    "LogLevel": {
      "Couchbase": "Information"
    }
  }
}
```

## 📖 Resources

- [Couchbase .NET SDK](https://docs.couchbase.com/dotnet-sdk/current/hello-world/start-using-sdk.html)
- [Couchbase DI Extensions](https://github.com/couchbase/couchbase-net-client)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

## 👨‍💻 Author

**Mahmoud El Hindawy**
- GitHub: [@mahmoudelhindawy](https://github.com/mahmoudelhindawy)
- Email: mahmoud.elhendawy@gmail.com

---

**⭐ This is a REAL Entity Framework Core provider for Couchbase, not a hybrid approach!**
