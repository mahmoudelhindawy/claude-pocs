# CouchDB with Entity Framework Core - Clean Architecture API

A complete .NET 8.0 Web API implementing **Clean Architecture** using **Entity Framework Core patterns** with **CouchDB** as the backend database.

## 🎯 Key Innovation

This project demonstrates a **hybrid approach** that combines:
- ✅ **Entity Framework Core patterns** (DbContext, DbSet, Repository)
- ✅ **CouchDB NoSQL database** for actual data storage
- ✅ **Clean Architecture principles** with proper separation of concerns

## 🏗️ Architecture Overview

```
┌─────────────────────────────────────────┐
│        WebApi (Presentation)            │
│   Controllers, DI, Program.cs           │
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
│  EF Core DbContext + CouchDB Client     │
└─────────────────────────────────────────┘
```

## 🌟 Why Entity Framework Core with CouchDB?

### Traditional Challenges:
1. **No Native EF Core Provider** - CouchDB doesn't have an official EF Core database provider
2. **Different Paradigms** - EF Core is designed for relational databases, CouchDB is document-based
3. **Limited Tooling** - Can't use EF Core migrations or LINQ providers with CouchDB

### Our Solution:
We created a **hybrid approach** that:
- ✅ Uses **EF Core patterns** (DbContext, DbSet, Repository pattern)
- ✅ Stores data in **CouchDB** via MyCouch client
- ✅ Provides **familiar EF Core API** for developers
- ✅ Maintains **Clean Architecture** principles
- ✅ Enables **LINQ queries** on the application layer

## 📁 Project Structure

```
CouchDbEFCore/
├── src/
│   ├── Domain/                           # Core Business Layer
│   │   ├── Entities/
│   │   │   ├── BaseEntity.cs            # Base entity with CouchDB fields
│   │   │   └── Product.cs               # Product entity
│   │   └── Interfaces/
│   │       ├── IRepository.cs           # Generic repository interface
│   │       └── IProductRepository.cs    # Product repository interface
│   │
│   ├── Application/                      # Business Logic Layer
│   │   ├── DTOs/
│   │   │   └── ProductDto.cs            # Data Transfer Objects
│   │   ├── Interfaces/
│   │   │   └── IProductService.cs       # Service interface
│   │   └── Services/
│   │       └── ProductService.cs        # Business logic implementation
│   │
│   ├── Infrastructure/                   # Data Access Layer
│   │   ├── Persistence/
│   │   │   ├── CouchDbContext.cs        # EF Core DbContext for CouchDB
│   │   │   └── CouchDbSettings.cs       # Configuration
│   │   └── Repositories/
│   │       ├── Repository.cs            # Generic EF Core repository
│   │       └── ProductRepository.cs     # Product repository
│   │
│   └── WebApi/                          # Presentation Layer
│       ├── Controllers/
│       │   └── ProductsController.cs    # REST API endpoints
│       ├── Program.cs                   # App configuration
│       ├── appsettings.json            # Settings
│       └── appsettings.Development.json
│
├── docker-compose.yml                   # CouchDB container
├── CouchDbEFCore.sln                   # Solution file
└── README.md                            # This file
```

## 🚀 Technologies Used

| Technology | Purpose |
|-----------|---------|
| **.NET 8.0** | Web API framework |
| **Entity Framework Core 8.0** | ORM patterns and abstractions |
| **CouchDB 3.3** | NoSQL document database |
| **MyCouch 8.1.0** | CouchDB client for .NET |
| **Swagger/OpenAPI** | API documentation |
| **Docker** | Container platform |

## 📋 Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Git](https://git-scm.com/downloads)
- Your favorite IDE (VS 2022, VS Code, Rider)

## 🔧 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/mahmoudelhindawy/claude-pocs.git
cd claude-pocs/CouchDbEFCore
```

### 2. Start CouchDB

```bash
docker-compose up -d
```

Verify CouchDB is running:
```bash
curl http://localhost:5984
```

Access Fauxton (CouchDB UI): http://localhost:5984/_utils

### 3. Restore Dependencies

```bash
dotnet restore
```

### 4. Build the Solution

```bash
dotnet build
```

### 5. Run the API

```bash
cd src/WebApi
dotnet run
```

The API will be available at:
- **HTTP:** http://localhost:5000
- **HTTPS:** https://localhost:5001
- **Swagger:** http://localhost:5000/swagger

## 📚 How It Works

### The Hybrid Approach Explained

#### 1. **Entity Framework Core DbContext**

Our `CouchDbContext` inherits from `DbContext` but uses CouchDB:

```csharp
public class CouchDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    
    // Provides access to CouchDB client
    public MyCouchClient GetCouchClient() { ... }
    
    // Uses InMemory provider for EF Core compatibility
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("CouchDbCache");
    }
}
```

**Why InMemory?**
- EF Core requires a database provider
- We use InMemory as a "cache layer"
- Actual persistence happens in CouchDB via MyCouch

#### 2. **Repository Pattern with CouchDB**

The `Repository<T>` class uses EF Core patterns but stores data in CouchDB:

```csharp
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly CouchDbContext _context;
    protected readonly DbSet<T> _dbSet;
    
    public virtual async Task<T> CreateAsync(T entity)
    {
        // Save to CouchDB
        var client = _context.GetCouchClient();
        var response = await client.Documents.PostAsync(json);
        
        // Track in EF Core DbSet for consistency
        _dbSet.Add(entity);
        
        return entity;
    }
}
```

**Benefits:**
- ✅ Familiar EF Core API
- ✅ CouchDB document storage
- ✅ Revision tracking (`_rev`)
- ✅ LINQ query support (in-memory)

#### 3. **Entity Configuration**

Entities use Data Annotations for CouchDB fields:

```csharp
public abstract class BaseEntity
{
    [Key]
    [Column("_id")]
    public string Id { get; set; }
    
    [Column("_rev")]
    public string? Rev { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

## 📊 API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/products` | Get all products |
| GET | `/api/products/{id}` | Get product by ID |
| GET | `/api/products/category/{category}` | Get products by category |
| GET | `/api/products/active` | Get active products |
| POST | `/api/products` | Create new product |
| PUT | `/api/products/{id}` | Update product |
| DELETE | `/api/products/{id}` | Delete product |

### Example Requests

#### Create a Product

```bash
curl -X POST "http://localhost:5000/api/products" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Gaming Laptop",
    "description": "High-performance gaming laptop",
    "price": 1299.99,
    "quantity": 15,
    "category": "Electronics"
  }'
```

#### Get All Products

```bash
curl http://localhost:5000/api/products
```

#### Response Example

```json
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "name": "Gaming Laptop",
    "description": "High-performance gaming laptop",
    "price": 1299.99,
    "quantity": 15,
    "category": "Electronics",
    "isActive": true,
    "createdAt": "2024-12-22T10:00:00Z",
    "updatedAt": null
  }
]
```

## 🔐 Configuration

### appsettings.json

```json
{
  "CouchDbSettings": {
    "Url": "http://localhost:5984",
    "DatabaseName": "products_db",
    "Username": "admin",
    "Password": "password"
  }
}
```

### Environment Variables

```bash
export CouchDbSettings__Url="http://localhost:5984"
export CouchDbSettings__DatabaseName="products_db"
export CouchDbSettings__Username="admin"
export CouchDbSettings__Password="password"
```

## 🎓 Key Concepts

### 1. **Clean Architecture Layers**

- **Domain**: Core entities and business rules (no dependencies)
- **Application**: Use cases and business logic (depends on Domain)
- **Infrastructure**: Data access and external services (depends on Domain)
- **WebApi**: REST API and UI concerns (depends on all)

### 2. **Entity Framework Core Patterns**

- **DbContext**: Database abstraction
- **DbSet<T>**: Collection of entities
- **Repository Pattern**: Data access abstraction
- **Unit of Work**: Transaction management via SaveChangesAsync()

### 3. **CouchDB Integration**

- **Document Storage**: JSON documents with `_id` and `_rev`
- **Revision Tracking**: Optimistic concurrency control
- **RESTful API**: HTTP-based document operations
- **Views**: MapReduce queries (can be added)

## 🏛️ Design Patterns

1. **Repository Pattern** - Abstract data access
2. **Service Pattern** - Encapsulate business logic
3. **DTO Pattern** - Separate API contracts from domain
4. **Dependency Injection** - Loose coupling
5. **Options Pattern** - Strongly-typed configuration
6. **Hybrid Pattern** - EF Core + CouchDB integration

## ✨ Advantages of This Approach

### For Developers:
- ✅ **Familiar API** - Use EF Core patterns you already know
- ✅ **LINQ Support** - Query data with LINQ (in-memory)
- ✅ **Testability** - Easy to mock repositories
- ✅ **Maintainability** - Clean separation of concerns

### For CouchDB:
- ✅ **NoSQL Benefits** - Flexible schema, horizontal scaling
- ✅ **Replication** - Built-in multi-master replication
- ✅ **Offline-First** - Perfect for mobile/edge scenarios
- ✅ **REST API** - Easy integration with other systems

### For Architecture:
- ✅ **Clean Code** - Follows SOLID principles
- ✅ **Testable** - Each layer independently testable
- ✅ **Extensible** - Easy to add new features
- ✅ **Technology Agnostic** - Can swap databases easily

## 🚧 Limitations & Trade-offs

### What Works:
- ✅ CRUD operations
- ✅ Simple filtering (LINQ in-memory)
- ✅ Repository pattern
- ✅ Unit of Work pattern
- ✅ Data annotations validation

### What Doesn't Work:
- ❌ EF Core Migrations (CouchDB is schema-less)
- ❌ Database-level LINQ queries (queries run in-memory)
- ❌ Navigation properties with lazy loading
- ❌ Complex joins (CouchDB is document-based)
- ❌ Stored procedures or functions

### Performance Considerations:
- Queries execute in-memory after loading from CouchDB
- For large datasets, implement CouchDB views
- Consider caching strategies for frequently accessed data

## 🧪 Testing

### Unit Testing

```csharp
public class ProductServiceTests
{
    [Fact]
    public async Task CreateProduct_ShouldReturnCreatedProduct()
    {
        // Arrange
        var mockRepo = new Mock<IProductRepository>();
        var service = new ProductService(mockRepo.Object);
        
        // Act
        var result = await service.CreateProductAsync(new CreateProductDto
        {
            Name = "Test Product",
            Price = 99.99m
        });
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Product", result.Name);
    }
}
```

## 📈 Extending the Application

### Adding a New Entity

1. **Create Entity** in `Domain/Entities/`
2. **Add DbSet** to `CouchDbContext`
3. **Create Repository Interface** in `Domain/Interfaces/`
4. **Implement Repository** in `Infrastructure/Repositories/`
5. **Create DTOs** in `Application/DTOs/`
6. **Create Service** in `Application/Services/`
7. **Create Controller** in `WebApi/Controllers/`
8. **Register Services** in `Program.cs`

### Adding CouchDB Views

Create views for efficient querying:

```javascript
// In CouchDB, create a design document
{
  "_id": "_design/products",
  "views": {
    "by_category": {
      "map": "function (doc) { if (doc.category) emit(doc.category, doc); }"
    }
  }
}
```

Query from repository:

```csharp
var query = new QueryViewRequest("products/by_category")
{
    Key = category,
    IncludeDocs = true
};
var response = await client.Views.QueryAsync<string, Product>(query);
```

## 🐳 Docker Deployment

Build and run everything with Docker:

```bash
# Build the API image
docker build -t couchdb-efcore-api .

# Run with docker-compose
docker-compose up -d

# Access the API
curl http://localhost:5000/api/products
```

## 🔍 Troubleshooting

### CouchDB Connection Issues

```bash
# Check if CouchDB is running
curl http://localhost:5984

# Check database exists
curl http://admin:password@localhost:5984/products_db

# View all databases
curl http://admin:password@localhost:5984/_all_dbs
```

### Application Errors

```bash
# Check logs
dotnet run --verbosity detailed

# Enable EF Core logging
# In appsettings.json, set "Microsoft.EntityFrameworkCore": "Debug"
```

## 📝 Best Practices

1. **Always include `_rev`** in update operations
2. **Use string IDs** (CouchDB standard)
3. **Implement conflict resolution** for multi-user scenarios
4. **Create indexes** for frequently queried fields
5. **Use views** for complex queries
6. **Enable replication** for high availability

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📚 Additional Resources

- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [CouchDB Official Documentation](https://docs.couchdb.org/)
- [MyCouch Library](https://github.com/danielwertheim/mycouch)
- [Clean Architecture by Uncle Bob](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

## 👨‍💻 Author

**Mahmoud El Hindawy**
- GitHub: [@mahmoudelhindawy](https://github.com/mahmoudelhindawy)
- Email: mahmoud.elhendawy@gmail.com
- Location: Riyadh, Saudi Arabia

## 📝 License

This project is licensed under the MIT License.

## 🙏 Acknowledgments

- Clean Architecture concepts by Robert C. Martin
- Entity Framework Core team
- CouchDB community
- MyCouch library maintainers
- Created with assistance from **Claude AI** by Anthropic

---

**⭐ If you find this project helpful, please star the repository!**

**Built with ❤️ combining Entity Framework Core patterns with CouchDB power**
