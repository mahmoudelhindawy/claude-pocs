# Couchbase with Entity Framework Core - Clean Architecture API

A complete .NET 8.0 Web API implementing **Clean Architecture** using **Entity Framework Core patterns** with **Couchbase** as the backend NoSQL database.

## 🎯 What is This Project?

This project demonstrates a **hybrid approach** combining:
- ✅ **Entity Framework Core patterns** (DbContext, DbSet, Repository)
- ✅ **Couchbase NoSQL database** for high-performance data storage
- ✅ **Clean Architecture principles** with proper layer separation
- ✅ **N1QL queries** for efficient data retrieval

## 🌟 Couchbase vs CouchDB

### Why Couchbase?

| Feature | Couchbase | CouchDB |
|---------|-----------|---------|
| **Performance** | High-performance memory-first | Disk-based storage |
| **Scaling** | Built for massive scale | Good for moderate scale |
| **Queries** | N1QL (SQL for JSON) | MapReduce views |
| **.NET SDK** | Official robust SDK | Third-party libraries |
| **Use Cases** | Enterprise apps, caching, session store | Mobile sync, offline-first |

**Couchbase is ideal for:**
- 🚀 High-throughput applications
- ⚡ Sub-millisecond latency requirements
- 📈 Massive scalability needs
- 🔍 Complex SQL-like queries on JSON
- 💼 Enterprise-grade features

## 🏗️ Architecture Overview

```
┌─────────────────────────────────────────┐
│        WebApi (Presentation)            │
│   Controllers, DI, Startup              │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│       Application (Business Logic)      │
│   Services, DTOs, Interfaces            │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│        Domain (Core Business)           │
│   Entities, Interfaces                  │
└─────────────────▲───────────────────────┘
                  │
┌─────────────────┴───────────────────────┐
│    Infrastructure (Data Access)         │
│  EF Core DbContext + Couchbase SDK      │
└─────────────────────────────────────────┘
```

## 📁 Project Structure

```
CouchbaseEFCore/
├── src/
│   ├── Domain/
│   │   ├── Entities/
│   │   │   ├── BaseEntity.cs        # Base with Couchbase CAS
│   │   │   └── Product.cs
│   │   └── Interfaces/
│   │       ├── IRepository.cs
│   │       └── IProductRepository.cs
│   ├── Application/
│   │   ├── DTOs/
│   │   ├── Interfaces/
│   │   └── Services/
│   ├── Infrastructure/
│   │   ├── Persistence/
│   │   │   ├── CouchbaseContext.cs  # EF Core + Couchbase
│   │   │   └── CouchbaseSettings.cs
│   │   └── Repositories/
│   │       ├── Repository.cs        # Generic repository
│   │       └── ProductRepository.cs # N1QL queries
│   └── WebApi/
│       ├── Controllers/
│       ├── Program.cs
│       └── appsettings.json
├── docker-compose.yml
└── README.md
```

## 🚀 Technologies Used

| Technology | Version | Purpose |
|-----------|---------|---------|
| .NET | 8.0 | Web API Framework |
| Entity Framework Core | 8.0 | ORM Patterns |
| Couchbase | 7.6 Community | NoSQL Database |
| Couchbase .NET SDK | 3.5.1 | Database Client |
| Docker | Latest | Containerization |

## 📋 Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- 4GB+ RAM (for Couchbase)
- Your favorite IDE

## 🔧 Getting Started

### 1. Start Couchbase Server

```bash
docker-compose up -d
```

Wait for Couchbase to start (~60 seconds), then access the Web Console:
- **URL:** http://localhost:8091
- **Username:** Administrator
- **Password:** password

### 2. Configure Couchbase (First Time Only)

1. Open http://localhost:8091
2. Click "Setup New Cluster"
3. Set cluster name: `couchbase-cluster`
4. Admin username: `Administrator`
5. Admin password: `password`
6. Click "Next" and accept defaults
7. Click "Finish" to complete setup

The application will automatically create the `products` bucket on first run.

### 3. Build and Run the API

```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run the API
cd src/WebApi
dotnet run
```

Access the API:
- **HTTP:** http://localhost:5000
- **HTTPS:** https://localhost:5001
- **Swagger:** http://localhost:5000/swagger
- **Health Check:** http://localhost:5000/health

## 📚 How It Works

### The Hybrid Approach

#### 1. **CouchbaseContext (EF Core DbContext)**

```csharp
public class CouchbaseContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    
    // Access Couchbase SDK
    public async Task<ICluster> GetClusterAsync() { ... }
    public async Task<IBucket> GetBucketAsync() { ... }
    public async Task<ICouchbaseCollection> GetCollectionAsync() { ... }
}
```

#### 2. **Repository with Couchbase SDK**

```csharp
public class Repository<T> : IRepository<T>
{
    public async Task<T> CreateAsync(T entity)
    {
        // Use Couchbase SDK for persistence
        var collection = await _context.GetCollectionAsync();
        var result = await collection.InsertAsync(entity.Id, entity);
        
        // Track in EF Core DbSet
        _dbSet.Add(entity);
        
        return entity;
    }
}
```

#### 3. **N1QL Queries**

```csharp
public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
{
    var query = @"
        SELECT META().id, * 
        FROM `products` 
        WHERE type = 'product' AND category = $category
    ";
    
    return await ExecuteQueryAsync(query);
}
```

### Key Concepts

**CAS (Compare And Swap)**
- Couchbase's optimistic locking mechanism
- Prevents concurrent update conflicts
- Automatically handled by the SDK

**Document Type**
- Each entity has a `type` field (e.g., "product")
- Used for filtering in N1QL queries
- Enables multi-model data in same bucket

**N1QL**
- SQL-like query language for JSON documents
- Powerful indexing and query optimization
- Familiar syntax for developers

## 📊 API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/products` | Get all products |
| GET | `/api/products/{id}` | Get by ID |
| GET | `/api/products/category/{category}` | Get by category |
| GET | `/api/products/active` | Get active products |
| GET | `/api/products/search/{term}` | Search by name |
| POST | `/api/products` | Create product |
| PUT | `/api/products/{id}` | Update product |
| DELETE | `/api/products/{id}` | Delete product |

### Example Requests

#### Create Product

```bash
curl -X POST "http://localhost:5000/api/products" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Gaming Laptop",
    "description": "High-performance gaming laptop with RTX 4080",
    "price": 1899.99,
    "quantity": 25,
    "category": "Electronics"
  }'
```

#### Search Products

```bash
curl "http://localhost:5000/api/products/search/laptop"
```

#### Get by Category

```bash
curl "http://localhost:5000/api/products/category/Electronics"
```

## 🔐 Configuration

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
export CouchbaseSettings__BucketName="products"
```

## ⚡ Couchbase Features Used

### 1. **Key-Value Operations**
- Fast document CRUD by key
- Sub-millisecond latency
- Atomic operations with CAS

### 2. **N1QL Queries**
- SQL-like syntax
- Complex filtering and sorting
- JOIN operations (if needed)

### 3. **Indexes**
- Primary index created automatically
- Add secondary indexes for performance:

```sql
CREATE INDEX idx_category ON `products`(category) WHERE type = 'product';
CREATE INDEX idx_name ON `products`(LOWER(name)) WHERE type = 'product';
```

### 4. **Scopes and Collections**
- Organize data within buckets
- Multi-tenancy support
- Better data isolation

## 🏛️ Design Patterns

1. **Repository Pattern** - Abstract data access
2. **Unit of Work** - Transaction management
3. **Service Pattern** - Business logic encapsulation
4. **DTO Pattern** - API contract separation
5. **Dependency Injection** - Loose coupling
6. **Hybrid Pattern** - EF Core + Couchbase

## ✨ Advantages

### Performance
- ⚡ In-memory caching
- 🚀 Sub-millisecond reads
- 📈 Horizontal scaling
- 💾 Automatic data distribution

### Developer Experience
- 🎯 Familiar EF Core patterns
- 🔍 SQL-like N1QL queries
- 🛠️ Comprehensive .NET SDK
- 📚 Excellent documentation

### Enterprise Features
- 🔒 RBAC (Role-Based Access Control)
- 🔄 Cross-datacenter replication
- 💪 99.999% availability
- 📊 Built-in analytics

## 🚧 Limitations

### What Works
- ✅ CRUD operations
- ✅ N1QL queries
- ✅ Optimistic locking (CAS)
- ✅ Indexes
- ✅ Transactions (within bucket)

### What Doesn't Work
- ❌ EF Core migrations
- ❌ Navigation properties with joins
- ❌ LINQ-to-Couchbase (queries run via N1QL)
- ❌ Lazy loading

### Performance Considerations
- Use indexes for frequently queried fields
- Leverage key-value operations when possible
- Cache frequently accessed data
- Monitor query performance with EXPLAIN

## 🧪 Testing

### Unit Testing Example

```csharp
[Fact]
public async Task CreateProduct_ShouldStoreInCouchbase()
{
    // Arrange
    var mockContext = new Mock<CouchbaseContext>();
    var repository = new ProductRepository(mockContext.Object);
    
    // Act
    var product = await repository.CreateAsync(new Product
    {
        Name = "Test Product",
        Price = 99.99m
    });
    
    // Assert
    Assert.NotNull(product.Id);
    Assert.True(product.Cas > 0);
}
```

## 📈 Extending the Application

### Add a Secondary Index

```sql
-- In Couchbase Query Workbench
CREATE INDEX idx_price ON `products`(price) WHERE type = 'product';
```

### Add Full-Text Search

Couchbase supports full-text search (FTS):

1. Create FTS index in Couchbase UI
2. Use SDK to query:

```csharp
var searchResult = await bucket.SearchQueryAsync(
    "products-fts",
    SearchQuery.MatchPhrase("gaming laptop")
);
```

### Add Analytics Query

```csharp
var analyticsResult = await cluster.AnalyticsQueryAsync<Product>(
    "SELECT * FROM products WHERE price > 1000"
);
```

## 🐳 Docker Production Deployment

```dockerfile
# Multi-stage build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "WebApi.dll"]
```

## 🔍 Monitoring

### Couchbase Web Console
- **URL:** http://localhost:8091
- Monitor cluster health
- View bucket statistics
- Analyze query performance

### Application Health Check
```bash
curl http://localhost:5000/health
```

## 📝 Best Practices

1. **Use CAS for Concurrency** - Always provide CAS on updates
2. **Create Indexes** - Index frequently queried fields
3. **Use Type Field** - Distinguish document types
4. **Batch Operations** - Group multiple operations
5. **Monitor Performance** - Use Couchbase built-in tools
6. **Connection Pooling** - Reuse cluster connections
7. **Error Handling** - Handle CasMismatch and DocumentNotFound

## 🤝 Contributing

1. Fork the repository
2. Create feature branch
3. Commit changes
4. Push to branch
5. Open Pull Request

## 👨‍💻 Author

**Mahmoud El Hindawy**
- GitHub: [@mahmoudelhindawy](https://github.com/mahmoudelhindawy)
- Email: mahmoud.elhendawy@gmail.com

## 📝 License

MIT License

## 🙏 Acknowledgments

- Entity Framework Core team
- Couchbase team and community
- Clean Architecture by Robert C. Martin
- Created with **Claude AI** by Anthropic

---

**⭐ Star this repo if you find it helpful!**

**Built with ❤️ combining EF Core patterns with Couchbase power**
