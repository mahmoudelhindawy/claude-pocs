# Claude POCs (Proof of Concepts)

A collection of proof-of-concept projects and demos created with Claude AI, showcasing various technologies, architectures, and best practices.

## 📚 Projects

### 1. [CouchDB Clean Architecture API](./CouchDbCleanArchitectureApi)

A complete .NET 8.0 Web API implementing Clean Architecture (Onion Architecture) with CouchDB integration using MyCouch library.

**Technologies:**
- .NET 8.0
- CouchDB
- MyCouch Library
- Clean Architecture
- Docker
- Swagger/OpenAPI

**Features:**
- Four-layer architecture (Domain, Application, Infrastructure, WebApi)
- Repository pattern with direct CouchDB client
- Service layer with DTOs
- Full CRUD operations
- Docker Compose setup for CouchDB
- Comprehensive documentation

[View Project →](./CouchDbCleanArchitectureApi)

---

### 2. [CouchDB with Entity Framework Core](./CouchDbEFCore) ⭐ NEW!

A .NET 8.0 Web API demonstrating how to use **Entity Framework Core patterns** with **CouchDB** as the backend database.

**Technologies:**
- .NET 8.0
- Entity Framework Core 8.0
- CouchDB
- MyCouch Library
- Clean Architecture
- Docker

**Features:**
- Hybrid approach: EF Core patterns + CouchDB storage
- DbContext and DbSet abstractions
- Repository pattern with EF Core compatibility
- LINQ query support (in-memory)
- Full CRUD with revision tracking
- Comprehensive documentation on the hybrid approach

**Why This Project?**
- ✅ Learn how to bridge EF Core and NoSQL databases
- ✅ Use familiar EF Core APIs with CouchDB
- ✅ Understand the trade-offs and benefits
- ✅ See a practical hybrid architecture implementation

[View Project →](./CouchDbEFCore)

---

## 🎯 Purpose

This repository serves as a collection of:
- **Learning Projects** - Exploring new technologies and patterns
- **Best Practices** - Demonstrating clean code and architecture principles
- **Quick References** - Ready-to-use templates and examples
- **Experimentation** - Testing ideas and approaches
- **Hybrid Solutions** - Combining different technologies creatively

## 🚀 Getting Started

Each project has its own comprehensive README with detailed instructions. Navigate to the project folder and follow the setup guide.

## 📋 Project Structure

```
claude-pocs/
├── CouchDbCleanArchitectureApi/    # .NET + CouchDB (MyCouch Direct)
├── CouchDbEFCore/                  # .NET + CouchDB (EF Core Patterns)
├── [Future Projects]/              # More projects to come
└── README.md                       # This file
```

## 🎓 What You'll Learn

### From CouchDbCleanArchitectureApi:
- Clean Architecture implementation
- Direct CouchDB integration with MyCouch
- Repository and Service patterns
- REST API best practices

### From CouchDbEFCore:
- Entity Framework Core patterns
- Hybrid ORM + NoSQL approach
- DbContext abstraction with CouchDB
- Trade-offs between patterns and databases

## 🤝 Contributing

These are personal learning projects, but feedback and suggestions are welcome! Feel free to:
- Open an issue for suggestions
- Fork and experiment with the code
- Share your improvements

## 👨‍💻 Author

**Mahmoud El Hindawy**
- GitHub: [@mahmoudelhindawy](https://github.com/mahmoudelhindawy)
- Email: mahmoud.elhendawy@gmail.com
- Location: Riyadh, Saudi Arabia

## 📝 License

MIT License - Feel free to use these projects for learning and reference.

## 🙏 Acknowledgments

All projects in this repository were created with assistance from **Claude AI** by Anthropic, demonstrating the power of AI-assisted development.

---

**⭐ If you find these projects helpful, please star the repository!**
