# CouchDB Clean Architecture API

A complete .NET 8.0 Web API implementation following Clean Architecture (Onion Architecture) principles, demonstrating CouchDB integration with proper separation of concerns.

## 🏗️ Architecture Overview

This project implements Clean Architecture with four distinct layers:

```
┌─────────────────────────────────────────┐
│        WebApi (Presentation)            │
│   Controllers, Middleware, DI Setup     │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│       Application (Business Logic)      │
│   Services, DTOs, Use Cases, Mappings   │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│        Domain (Core Business)           │
│   Entities, Interfaces, Domain Logic    │
└─────────────────▲───────────────────────┘
                  │
┌─────────────────┴───────────────────────┐
│    Infrastructure (External Concerns)   │
│  Database Access, CouchDB, Persistence  │
└─────────────────────────────────────────┘
```

### Layer Responsibilities

#### Domain Layer (Core)
- Contains business entities and core business logic
- Defines repository interfaces
- No dependencies on other layers
- Pure business rules and domain models

#### Application Layer
- Implements business use cases
- Contains DTOs for data transfer
- Defines service interfaces
- Orchestrates data flow
- Depends only on Domain layer

#### Infrastructure Layer
- Implements data access logic
- Contains CouchDB-specific code
- Implements repository interfaces from Domain
- Handles database connections and queries
- Depends on Domain layer

#### WebApi Layer
- REST API endpoints
- Request/Response handling
- Dependency injection configuration
- API documentation (Swagger)
- Depends on all other layers

## 📁 Project Structure

```
CouchDbCleanArchitectureApi/
├── src/
│   ├── Domain/
│   │   ├── Entities/
│   │   │   ├── BaseEntity.cs          # Base class with common properties
│   │   │   └── Product.cs             # Product domain entity
│   │   └── Interfaces/
│   │       ├── IRepository.cs         # Generic repository interface
│   │       └── IProductRepository.cs  # Product-specific repository
│   │
│   ├── Application/
│   │   ├── DTOs/
│   │   │   └── ProductDto.cs          # Data transfer objects
│   │   ├── Interfaces/
│   │   │   └── IProductService.cs     # Service interface
│   │   └── Services/
│   │       └── ProductService.cs      # Business logic implementation
│   │
│   ├── Infrastructure/
│   │   ├── Persistence/
│   │   │   ├── CouchDbContext.cs      # Database context
│   │   │   └── CouchDbSettings.cs     # Configuration model
│   │   └── Repositories/
│   │       ├── CouchDbRepository.cs   # Generic repository implementation
│   │       └── ProductRepository.cs   # Product repository implementation
│   │
│   └── WebApi/
│       ├── Controllers/
│       │   └── ProductsController.cs  # REST API endpoints
│       ├── Program.cs                 # Application entry point
│       ├── appsettings.json          # Configuration
│       └── appsettings.Development.json
│
├── docker-compose.yml                 # CouchDB container setup
├── CouchDbCleanArchitectureApi.sln   # Solution file
└── README.md                          # This file
```

## 🚀 Technologies Used

- **.NET 8.0** - Latest LTS version with modern C# features
- **CouchDB 3.3** - NoSQL document database
- **MyCouch 8.1.0** - Official CouchDB client for .NET
- **ASP.NET Core** - Web framework for building APIs
- **Swagger/OpenAPI** - API documentation and testing
- **Docker** - Container platform for CouchDB

## 📋 Prerequisites

Before you begin, ensure you have the following installed:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (for running CouchDB)
- [Git](https://git-scm.com/downloads)
- A code editor ([Visual Studio 2022](https://visualstudio.microsoft.com/), [VS Code](https://code.visualstudio.com/), or [Rider](https://www.jetbrains.com/rider/))

## 🔧 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/mahmoudelhindawy/claude-pocs.git
cd claude-pocs/CouchDbCleanArchitectureApi
```

### 2. Start CouchDB with Docker

```bash
docker-compose up -d
```

This will:
- Pull the CouchDB 3.3 image
- Start CouchDB on port 5984
- Create a persistent volume for data
- Set up admin credentials (username: `admin`, password: `password`)

Verify CouchDB is running:
```bash
curl http://localhost:5984
```

Access CouchDB Admin UI: http://localhost:5984/_utils

### 3. Restore NuGet Packages

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
- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001
- Swagger UI: http://localhost:5000/swagger

## 📚 API Endpoints

### Products API

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/products` | Get all products |
| GET | `/api/products/{id}` | Get product by ID |
| GET | `/api/products/category/{category}` | Get products by category |
| GET | `/api/products/active` | Get all active products |
| POST | `/api/products` | Create new product |
| PUT | `/api/products/{id}` | Update existing product |
| DELETE | `/api/products/{id}` | Delete product |

### Example Requests

#### Create a Product

```bash
curl -X POST "http://localhost:5000/api/products" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Laptop",
    "description": "High-performance laptop",
    "price": 999.99,
    "quantity": 10,
    "category": "Electronics"
  }'
```

#### Get All Products

```bash
curl http://localhost:5000/api/products
```

#### Get Product by ID

```bash
curl http://localhost:5000/api/products/{id}
```

#### Update Product

```bash
curl -X PUT "http://localhost:5000/api/products/{id}" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Gaming Laptop",
    "description": "High-end gaming laptop",
    "price": 1299.99,
    "quantity": 5,
    "category": "Electronics",
    "isActive": true
  }'
```

#### Delete Product

```bash
curl -X DELETE "http://localhost:5000/api/products/{id}"
```

## 🔐 Configuration

### CouchDB Settings

Edit `src/WebApi/appsettings.json`:

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

You can also use environment variables:

```bash
export CouchDbSettings__Url="http://localhost:5984"
export CouchDbSettings__DatabaseName="products_db"
export CouchDbSettings__Username="admin"
export CouchDbSettings__Password="password"
```

## 📊 CouchDB Document Structure

Products are stored as JSON documents in CouchDB:

```json
{
  "_id": "unique-guid",
  "_rev": "1-revision-hash",
  "name": "Product Name",
  "description": "Product Description",
  "price": 99.99,
  "quantity": 50,
  "category": "Category Name",
  "isActive": true,
  "createdAt": "2024-12-21T10:00:00Z",
  "updatedAt": "2024-12-21T12:00:00Z"
}
```

## 🏛️ Design Patterns Used

1. **Repository Pattern** - Abstracts data access logic
2. **Service Pattern** - Encapsulates business logic
3. **DTO Pattern** - Separates API contracts from domain models
4. **Dependency Injection** - Loose coupling and testability
5. **Factory Pattern** - Object creation abstraction
6. **Options Pattern** - Strongly-typed configuration

## 🧪 Testing

### Manual Testing with Swagger

1. Run the application
2. Navigate to http://localhost:5000/swagger
3. Try out the API endpoints interactively

### Testing with curl

See the example requests in the API Endpoints section above.

### Unit Testing (Future Enhancement)

```bash
# To be implemented
dotnet test
```

## 🚢 Deployment

### Docker Deployment

Create a `Dockerfile` in the WebApi project:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/WebApi/WebApi.csproj", "WebApi/"]
COPY ["src/Application/Application.csproj", "Application/"]
COPY ["src/Domain/Domain.csproj", "Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore "WebApi/WebApi.csproj"
COPY src/ .
WORKDIR "/src/WebApi"
RUN dotnet build "WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApi.dll"]
```

Then build and run:

```bash
docker build -t couchdb-api .
docker run -p 5000:80 couchdb-api
```

### Production Considerations

1. **Security**
   - Use strong passwords for CouchDB
   - Enable HTTPS in production
   - Implement authentication/authorization
   - Use secrets management (Azure Key Vault, AWS Secrets Manager)

2. **Performance**
   - Enable CouchDB caching
   - Implement response caching
   - Use connection pooling
   - Consider read replicas for scaling

3. **Monitoring**
   - Add Application Insights or similar
   - Implement health checks
   - Set up logging (Serilog, NLog)
   - Monitor CouchDB performance

## 📈 Extending the Application

### Adding a New Entity

1. **Create Entity** in `Domain/Entities/YourEntity.cs`
2. **Create Repository Interface** in `Domain/Interfaces/IYourEntityRepository.cs`
3. **Create DTOs** in `Application/DTOs/YourEntityDto.cs`
4. **Create Service Interface** in `Application/Interfaces/IYourEntityService.cs`
5. **Implement Service** in `Application/Services/YourEntityService.cs`
6. **Implement Repository** in `Infrastructure/Repositories/YourEntityRepository.cs`
7. **Create Controller** in `WebApi/Controllers/YourEntitiesController.cs`
8. **Register Services** in `Program.cs`

### Adding Business Logic

Add methods to the service layer that orchestrate multiple repository calls or implement complex business rules.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 👨‍💻 Author

**Mahmoud El Hindawy**
- GitHub: [@mahmoudelhindawy](https://github.com/mahmoudelhindawy)
- Email: mahmoud.elhendawy@gmail.com

## 🙏 Acknowledgments

- Clean Architecture concepts by Robert C. Martin (Uncle Bob)
- ASP.NET Core team for the excellent framework
- CouchDB community for the great database
- MyCouch library maintainers

## 📚 Additional Resources

- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [CouchDB Documentation](https://docs.couchdb.org/)
- [ASP.NET Core Best Practices](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/best-practices)

---

**Built with ❤️ using Clean Architecture principles**
