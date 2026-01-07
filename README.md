# Forum API - Microservices Architecture

A modern forum application built with .NET 10.0 using microservices architecture, implementing Domain-Driven Design (DDD) principles, CQRS pattern, and event-driven communication.

## 📋 Overview

This project is a distributed forum system composed of three main microservices that handle different aspects of the application:

- **Forum Service**: Manages questions, answers, and forum-related operations
- **User Service**: Handles authentication, authorization, and user management
- **Notification Service**: Processes and delivers notifications to users

## 🏗️ Architecture

The project follows a microservices architecture with the following characteristics:

- **Domain-Driven Design (DDD)**: Each service is organized around business domains
- **Clean Architecture**: Clear separation of concerns with distinct layers (Domain, Application, Infrastructure, Endpoints)
- **Event-Driven Communication**: Microservices communicate through integration events
- **CQRS Pattern**: Separation of read and write operations
- **Minimal API**: Using .NET's minimal API approach for lightweight endpoints

### Building Blocks

The project includes a shared `BuildingBlocks` library that provides:

- **Base Domain Classes**: Aggregate, Entity, ValueObject, UniqueEntityId, WatchedList
- **Event Bus**: Implementation for both in-memory and RabbitMQ-based event publishing
- **Custom Exceptions**: Domain-specific exception handling (BadRequest, NotFound, Validation, etc.)
- **Middleware**: Correlation ID tracking, Request/Response logging
- **Logging**: Structured logging with Serilog
- **Pagination**: Generic pagination support
- **Result Pattern**: Functional error handling

## 🚀 Technologies

### Core Framework
- **.NET 10.0** - Latest .NET framework
- **C# 13** - Modern C# features
- **ASP.NET Core** - Web framework with Minimal APIs

### Databases
- **PostgreSQL** - Primary database for all services
- **Entity Framework Core 10.0** - ORM for database access
- **Npgsql** - PostgreSQL provider for EF Core

### Message Broker
- **RabbitMQ** - Message broker for asynchronous communication between services
- **RabbitMQ.Client 6.8.1** - RabbitMQ client library

### Authentication & Security
- **ASP.NET Core Identity** - User management framework
- **JWT Bearer Authentication** - Token-based authentication
- **Microsoft.IdentityModel.Tokens** - JWT token handling

### Logging & Monitoring
- **Serilog** - Structured logging
- **Serilog.AspNetCore** - ASP.NET Core integration
- **Serilog.Sinks.Console** - Console output
- **Serilog.Sinks.File** - File-based logging
- **Serilog.Expressions** - Advanced logging expressions

### API Documentation
- **Swagger/OpenAPI** - API documentation and testing interface
- **Swashbuckle.AspNetCore** - Swagger generator for ASP.NET Core

### DevOps & Containerization
- **Docker** - Container platform
- **Docker Compose** - Multi-container orchestration

### Testing
- **xUnit** - Testing framework
- **In-Memory Database** - For unit and integration testing

## 📊 Database Schema

Each microservice has its own dedicated database:

- **ForumDb**: Stores questions, answers, attachments, and forum-related data
- **UserDb**: Manages user accounts, roles, and authentication data
- **NotificationDb**: Handles notification records and delivery status

## 🔌 Port Mapping

### Microservices

| Service       | Local HTTP | Local HTTPS | Docker HTTP | Docker HTTPS | Docker Internal |
|---------------|------------|-------------|-------------|--------------|-----------------|
| Forum         | 5000       | 5050        | 8000        | 8443         | 8080 - 8081     |
| Notification  | 5001       | 5051        | 8001        | 8444         | 8080 - 8081     |
| User          | 5002       | 5052        | 8002        | 8445         | 8080 - 8081     |

**Note**: Docker uses safe ports (8000-8002, 8443-8445) to avoid browser blocking issues. Ports 6000-6007 are considered unsafe by modern browsers.

### Infrastructure

| Service        | Port  | Management Port |
|----------------|-------|-----------------|
| ForumDb        | 5433  | -               |
| NotificationDb | 5434  | -               |
| UserDb         | 5435  | -               |
| RabbitMQ       | 5672  | 15672           |

## 🎯 Features

### Forum Service
- Create, read, update, and delete questions
- Create, read, update, and delete answers
- Mark best answer for questions
- Filter questions by author
- Attachment management
- Student and Instructor roles
- Pagination support

### User Service
- User registration
- User authentication with JWT tokens
- Role-based authorization
- Password management
- Email validation
- Token refresh mechanism

### Notification Service
- Send notifications to users
- Read notification status
- Integration with other services via events
- Notification history

## 📂 Project Structure

```
Forum/
├── src/
│   ├── BuildingBlocks/          # Shared library
│   │   ├── Base/                # Domain base classes
│   │   ├── Exceptions/          # Custom exceptions
│   │   ├── Extensions/          # Helper extensions
│   │   ├── Logging/             # Logging infrastructure
│   │   ├── Messaging/           # Event bus implementation
│   │   └── Middleware/          # Common middleware
│   ├── Forum/                   # Forum microservice
│   │   ├── Application/         # Application layer
│   │   ├── Domain/              # Domain layer
│   │   ├── Endpoints/           # API endpoints
│   │   ├── Infrastructure/      # Infrastructure layer
│   │   └── Migrations/          # Database migrations
│   ├── Notification/            # Notification microservice
│   │   ├── Application/
│   │   ├── Domain/
│   │   ├── Endpoints/
│   │   ├── Infrastructure/
│   │   └── Migrations/
│   └── User/                    # User microservice
│       ├── Data/                # Data layer
│       ├── DTOs/                # Data transfer objects
│       ├── Endpoints/           # API endpoints
│       ├── Extensions/          # Extensions
│       ├── Migrations/          # Database migrations
│       └── Services/            # Business services
├── tests/
│   ├── Forum.UnitTest/          # Forum unit tests
│   ├── Notification.UnitTest/   # Notification unit tests
│   ├── User.UnitTest/           # User unit tests
│   └── Seed/                    # Database seeding
├── docs/                        # Documentation
├── compose.yaml                 # Docker Compose configuration
└── Forum.sln                    # Solution file
```

## 🚦 Getting Started

### Prerequisites

- .NET 10.0 SDK
- Docker and Docker Compose
- PostgreSQL (if running locally without Docker)
- RabbitMQ (if running locally without Docker)

### Running with Docker Compose

1. Clone the repository:
```bash
git clone <repository-url>
cd Forum
```

2. **Option A - Run Everything in Docker (Recommended)**
```bash
# Start all services (APIs + Databases + RabbitMQ)
docker-compose up -d

# Access the APIs:
# Forum API:        http://localhost:8000/swagger
# Notification API: http://localhost:8001/swagger
# User API:         http://localhost:8002/swagger

# HTTPS is also available:
# Forum API:        https://localhost:8443/swagger
# Notification API: https://localhost:8444/swagger
# User API:         https://localhost:8445/swagger
```

3. **Option B - Run APIs Locally (For Debugging)**
```bash
# Start only infrastructure (databases and RabbitMQ)
docker-compose up -d userdb forumdb notificationdb messagebroker

# Terminal 1 - Forum Service
cd src/Forum
dotnet run

# Terminal 2 - User Service
cd src/User
dotnet run

# Terminal 3 - Notification Service
cd src/Notification
dotnet run

# Access the APIs:
# Forum API:        http://localhost:5000/swagger
# Notification API: http://localhost:5001/swagger
# User API:         http://localhost:5002/swagger
```

### HTTPS Certificates in Docker

HTTPS certificates are automatically generated inside containers on first startup. No manual configuration needed!

### Database Migrations

Migrations run automatically when starting services. To run manually:

```bash
# Forum Service
cd src/Forum
dotnet ef database update

# User Service
cd src/User
dotnet ef database update

# Notification Service
cd src/Notification
dotnet ef database update
```

## 📖 API Documentation

### Local Development

- **Forum API**: http://localhost:5000/swagger
- **User API**: http://localhost:5002/swagger
- **Notification API**: http://localhost:5001/swagger

### Docker Environment

**HTTP:**
- **Forum API**: http://localhost:8000/swagger
- **User API**: http://localhost:8002/swagger
- **Notification API**: http://localhost:8001/swagger

**HTTPS:**
- **Forum API**: https://localhost:8443/swagger
- **User API**: https://localhost:8445/swagger
- **Notification API**: https://localhost:8444/swagger

### RabbitMQ Management

- **URL**: http://localhost:15672
- **Username**: guest
- **Password**: guest

## 🧪 Testing

Run all tests:
```bash
dotnet test
```

Run tests for a specific project:
```bash
dotnet test tests/Forum.UnitTest
dotnet test tests/User.UnitTest
dotnet test tests/Notification.UnitTest
```

## 🔧 Configuration

Each service can be configured via `appsettings.json` and `appsettings.Development.json`. Key configurations include:

- **Connection Strings**: Database connections
- **JWT Settings**: Token configuration (User service)
- **RabbitMQ Settings**: Message broker configuration
- **Logging Levels**: Serilog configuration
- **CORS Policies**: Cross-origin resource sharing

## 📝 Domain Models

### Forum Service
- **Question**: Forum questions with title, content, and metadata
- **Answer**: Responses to questions
- **Attachment**: File attachments for questions/answers
- **Student**: Student user type
- **Instructor**: Instructor user type with additional privileges

### User Service
- **ApplicationUser**: Extended Identity user
- **Roles**: Role-based access control

### Notification Service
- **Notification**: User notifications with read status

## 🔄 Event-Driven Communication

The system uses an event bus pattern for inter-service communication:

- **Domain Events**: Events within a single service boundary
- **Integration Events**: Events that cross service boundaries
- **Event Handlers**: Process events asynchronously

Supported event bus implementations:
- **InMemoryEventBus**: For development and testing
- **RabbitMQEventBus**: For production use

## 🛡️ Error Handling

The application implements comprehensive error handling with:

- **Custom Exception Types**: BadRequest, NotFound, Validation, Forbidden, InternalServer
- **Global Exception Handler**: Centralized exception processing
- **Result Pattern**: Functional error handling for operations
- **Structured Logging**: All errors are logged with context

## 🐳 Docker

### Quick Commands

```bash
# Start everything
docker-compose up -d

# View logs
docker-compose logs -f

# View specific service logs
docker-compose logs -f forum
docker-compose logs -f notification
docker-compose logs -f user

# Check status
docker-compose ps

# Stop everything
docker-compose down

# Stop and remove volumes (clean slate)
docker-compose down -v

# Restart a specific service
docker-compose restart forum
```

### Docker Features

✅ **Automatic HTTPS Certificates** - Generated automatically inside containers  
✅ **Safe Ports** - Uses browser-safe ports (8000-8002, 8443-8445)  
✅ **Health Checks** - All services have health monitoring  
✅ **Persistent Data** - Databases use volumes for data persistence  
✅ **Auto-restart** - Services restart automatically on failure  
✅ **Zero Configuration** - Just run `docker-compose up -d`

### Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                        Host Machine                         │
│                                                             │
│  Browser → :8000 (Forum HTTP)    → :8443 (Forum HTTPS)     │
│         → :8001 (Notification)   → :8444 (Notification)     │
│         → :8002 (User)          → :8445 (User)             │
│         → :15672 (RabbitMQ UI)                             │
│                                                             │
│  ┌──────────────────────────────────────────────────────┐  │
│  │              Docker Network                          │  │
│  │                                                      │  │
│  │  ┏━━━━━━━━━━┓  ┏━━━━━━━━━━━━━┓  ┏━━━━━━━━━━┓       │  │
│  │  ┃ Forum    ┃  ┃ Notification┃  ┃  User    ┃       │  │
│  │  ┃   API    ┃  ┃     API     ┃  ┃   API    ┃       │  │
│  │  ┗━━━━━━━━━━┛  ┗━━━━━━━━━━━━━┛  ┗━━━━━━━━━━┛       │  │
│  │       ↓              ↓                ↓              │  │
│  │  ┌────────┐    ┌────────┐      ┌────────┐          │  │
│  │  │ForumDB │    │NotifyDB│      │UserDB  │          │  │
│  │  └────────┘    └────────┘      └────────┘          │  │
│  │       ↓              ↓                ↓              │  │
│  │            ┌───────────────────┐                     │  │
│  │            │    RabbitMQ       │                     │  │
│  │            └───────────────────┘                     │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

### Troubleshooting

**APIs not responding?**
```bash
# Check if containers are running
docker-compose ps

# Check logs for errors
docker-compose logs -f
```

**Database connection issues?**
```bash
# Restart databases
docker-compose restart forumdb notificationdb userdb
```

**RabbitMQ issues?**
```bash
# Restart RabbitMQ
docker-compose restart messagebroker
```

**Clean everything and start fresh?**
```bash
docker-compose down -v
docker-compose up -d --build
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙏 Acknowledgments

- Built with .NET 10.0
- Inspired by Clean Architecture and DDD principles
- Microservices architecture patterns

