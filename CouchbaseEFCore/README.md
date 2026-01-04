# Couchbase with Entity Framework Core - Official Provider

A complete .NET 8.0 Web API using the **official Couchbase.EntityFrameworkCore provider** with Clean Architecture.

## ✨ What's Special About This Project

This project uses the **official Couchbase Entity Framework Core provider** (`Couchbase.EntityFrameworkCore`), which provides:

- ✅ **True EF Core Integration** - Not a workaround or hybrid approach
- ✅ **LINQ Queries** - Automatically translated to N1QL
- ✅ **All EF Core Features** - Migrations, change tracking, async operations
- ✅ **Official Support** - Maintained by Couchbase
- ✅ **Clean Architecture** - Proper layer separation

**Official Documentation:** https://docs.couchbase.com/efcore-provider/current/entity-framework-core-configuration.html

---

## 🏗️ Architecture

```
┌─────────────────────────────────┐
│   WebApi (Presentation)         │
│   - REST Controllers            │
│   - Swagger UI                  │
└────────────┬────────────────────┘
             │
┌────────────▼────────────────────┐
│   Application (Business Logic)  │
│   - Services                    │
│   - DTOs                        │
└────────────┬────────────────────┘
             │
┌────────────▼────────────────────┐
│   Domain (Core Business)        │
│   - Entities                    │
│   - Interfaces                  │
└────────────▲────────────────────┘
             │
┌────────────┴────────────────────┐
│   Infrastructure (Data Access)  │
│   - EF Core DbContext           │
│   - Repositories                │
└────────────┬────────────────────┘
             │
┌────────────▼────────────────────┐
│   Couchbase Server              │
│   - Official EF Core Provider   │
└─────────────────────────────────┘
```

---

## 🚀 Technologies

| Technology | Version | Purpose |
|-----------|---------|---------|
| .NET | 8.0 | Web API Framework |
| Entity Framework Core | 8.0 | ORM |
| **Couchbase.EntityFrameworkCore** | 1.0.0 | Official EF Core Provider |
| Couchbase Server | 7.6 Community | NoSQL Database |
| Docker | Latest | Containerization |

---

## 📋 Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- 4GB+ RAM

---

## 🔧 Getting Started

### 1. Start Couchbase Server

```bash
docker-compose up -d
```

Wait ~60 seconds for Couchbase to start, then configure it:

1. Open http://localhost:8091
2. Click "Setup New Cluster"
3. Cluster Name: `couchbase-cluster`
4. Username: `Administrator`
5. Password: `password`
6. Click "Finish"

The application will automatically create the `products` bucket.

### 2. Run the API

```bash
cd src/WebApi
dotnet restore
dotnet build
dotnet run
```

### 3. Access the Application

- **Swagger UI:** http://localhost:5000
- **API:** http://localhost:5000/api/products
- **Health Check:** http://localhost:5000/health

---

## 🎯 Key Features

### 1. Official Couchbase Provider

```csharp
// Simple, one-line registration
builder.Services.AddDbContext<CouchbaseContext>(options =>
    options.UseCouchbase(connectionString, username, password));
```

### 2. Entity Configuration

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Product>(entity =>
    {
        // Specify Couchbase bucket
        entity.ToCouchbaseBucket("products");
        
        // Standard EF Core configuration
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
        // ... more configurations
    });
}
```

### 3. LINQ Queries Work Natively

```csharp
// LINQ is automatically translated to N1QL!
public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
{
    return await _dbSet
        .Where(p => p.Category == category)
        .ToListAsync();
}
```

### 4. Standard EF Core Repository

```csharp
public async Task<T> CreateAsync(T entity)
{
    // Just use standard EF Core methods
    await _dbSet.AddAsync(entity);
    await _context.SaveChangesAsync();
    return entity;
}
```

---

## 📚 API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/products` | Get all products |
| GET | `/api/products/{id}` | Get by ID |
| GET | `/api/products/category/{category}` | Get by category (LINQ) |
| GET | `/api/products/active` | Get active products (LINQ) |
| GET | `/api/products/search/{term}` | Search by name (LINQ) |
| POST | `/api/products` | Create product |
| PUT | `/api/products/{id}` | Update product |
| DELETE | `/api/products/{id}` | Delete product |

---

## 🧪 Testing

### Create a Product

```bash
curl -X POST http://localhost:5000/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Gaming Laptop",
    "description": "High-performance gaming laptop",
    "price": 1499.99,
    "quantity": 10,
    "category": "Electronics"
  }'
```

### Get All Products

```bash
curl http://localhost:5000/api/products
```

### Search by Name

```bash
curl http://localhost:5000/api/products/search/laptop
```

### Get by Category

```bash
curl http://localhost:5000/api/products/category/Electronics
```

---

## ⚙️ Configuration

### appsettings.json

```json
{
  "CouchbaseSettings": {
    "ConnectionString": "couchbase://localhost",
    "Username": "Administrator",
    "Password": "password",
    "BucketName": "products"
  }
}
```

### Environment Variables

```bash
export CouchbaseSettings__ConnectionString="couchbase://localhost"
export CouchbaseSettings__Username="Administrator"
export CouchbaseSettings__Password="password"
export CouchbaseSettings__BucketName="products"
```

---

## 📁 Project Structure

```
CouchbaseEFCore/
├── src/
│   ├── Domain/                    # Core business entities
│   │   ├── Entities/
│   │   └── Interfaces/
│   ├── Application/               # Business logic
│   │   ├── DTOs/
│   │   ├── Services/
│   │   └── Interfaces/
│   ├── Infrastructure/            # Data access
│   │   ├── Persistence/
│   │   │   ├── CouchbaseContext.cs    # EF Core DbContext
│   │   │   └── CouchbaseSettings.cs
│   │   └── Repositories/
│   │       ├── Repository.cs          # Generic repository
│   │       └── ProductRepository.cs   # Product-specific queries
│   └── WebApi/                    # REST API
│       ├── Controllers/
│       ├── Program.cs
│       └── appsettings.json
├── docker-compose.yml
└── README.md
```

---

## ✨ Benefits of Official Provider

### vs Custom/Manual Approach

| Feature | Custom Approach | Official Provider |
|---------|----------------|-------------------|
| **Package** | Multiple packages | Single package ✅ |
| **Setup** | Complex DI configuration | One-line registration ✅ |
| **Code Lines** | ~500 lines | ~200 lines ✅ |
| **LINQ Support** | Manual N1QL queries | Native LINQ ✅ |
| **Maintainability** | Difficult | Easy ✅ |
| **Official Support** | No | Yes ✅ |
| **EF Core Features** | Limited | All features ✅ |

---

## 🎓 What You'll Learn

From this project:
- ✅ How to use the official Couchbase EF Core provider
- ✅ Clean Architecture with NoSQL databases
- ✅ LINQ to N1QL translation
- ✅ Standard EF Core patterns with Couchbase
- ✅ Repository pattern with EF Core
- ✅ Dependency injection best practices

---

## 🔍 Troubleshooting

### Couchbase Connection Issues

```bash
# Check if Couchbase is running
docker ps | grep couchbase

# View Couchbase logs
docker logs couchbase-server

# Test connection
curl http://localhost:8091
```

### Application Errors

Check the application logs for detailed error messages. Common issues:

1. **Bucket doesn't exist** - Create it manually in Couchbase UI
2. **Connection refused** - Ensure Couchbase is running
3. **Authentication failed** - Check username/password in appsettings.json

---

## 📖 Resources

- [Official Couchbase EF Core Documentation](https://docs.couchbase.com/efcore-provider/current/entity-framework-core-configuration.html)
- [Couchbase .NET SDK Documentation](https://docs.couchbase.com/dotnet-sdk/current/hello-world/start-using-sdk.html)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [Clean Architecture by Uncle Bob](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

---

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Open a Pull Request

---

## 👨‍💻 Author

**Mahmoud El Hindawy**
- GitHub: [@mahmoudelhindawy](https://github.com/mahmoudelhindawy)
- Email: mahmoud.elhendawy@gmail.com
- Location: Riyadh, Saudi Arabia

---

## 📝 License

MIT License - Feel free to use for learning and reference.

---

## 🙏 Acknowledgments

- Couchbase team for the official EF Core provider
- Entity Framework Core team
- Clean Architecture by Robert C. Martin
- Created with **Claude AI** by Anthropic

---

**⭐ If you find this project helpful, please star the repository!**

**Built with the official Couchbase.EntityFrameworkCore provider** 🚀
