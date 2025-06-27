# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

TaskMaster is an AI-powered educational platform for language learning and assignment management. It's a modular monolith built with .NET 9.0, implementing Clean Architecture principles with CQRS, event-driven patterns, and multi-module design.

## Common Commands

### Building and Running
```bash
# Build entire solution
dotnet build TaskMaster.sln

# Run the main application
dotnet run --project TaskMaster.Bootstrapper

# Run with specific environment
dotnet run --project TaskMaster.Bootstrapper --environment Development
```

### Database Operations
```bash
# Add new migration (replace ModuleName and MigrationName)
dotnet ef migrations add MigrationName --project Modules/ModuleName/TaskMaster.Modules.ModuleName --context ModuleNameDbContext

# Update database for specific module
dotnet ef database update --project Modules/Accounts/TaskMaster.Modules.Accounts --context UsersDbContext
dotnet ef database update --project Modules/Teaching/TaskMaster.Modules.Teaching --context TeachingDbContext
dotnet ef database update --project Modules/Exercises/TaskMaster.Modules.Exercises --context ExercisesDbContext
```

### Testing
```bash
# Run all tests
dotnet test

# Run tests for specific module
dotnet test Modules/Teaching/TaskMaster.Modules.Teaching.Tests

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Infrastructure Services
```bash
# Start required services (PostgreSQL and Redis)
docker-compose up -d

# Stop services
docker-compose down
```

## Architecture Overview

### Modular Structure
- **Infra/** - Core abstractions, infrastructure services, shared models
- **Modules/** - Business domains (Accounts, Exercises, Teaching, AI)
- **TaskMaster.Bootstrapper/** - Main application entry point
- **TasMaster.Queries/** - Shared query definitions
- **TaskMaster.Models.Teaching/** - Teaching domain models

### Key Patterns
- **CQRS**: Commands and Queries are separated with dedicated handlers
- **Event-Driven**: In-memory event bus with background processing using .NET Channels
- **Module Independence**: Each module has its own DbContext and service registration
- **Clean Architecture**: Dependencies flow inward, domain logic is isolated

### Database Design
- **Multi-schema approach**: Each module uses separate database schema
- **Schema naming**: Users (Accounts), Teaching (Teaching), Exercises (Exercises)
- **Code-First**: EF Core migrations per module

### Module Communication
- **Cross-module queries**: Use shared query handlers in external folders
- **Events**: Modules communicate via domain events
- **No direct references**: Modules don't directly reference each other's internals

## Configuration Requirements

### appsettings.json Structure
```json
{
  "AuthSettings": {
    "IssuerSigningKey": "your-jwt-signing-key",
    "Issuer": "TaskMaster",
    "Expiry": "01:00:00"
  },
  "OpenAiSettings": {
    "apiKey": "your-openai-api-key"
  },
  "PostgresSettings": {
    "connectionString": "Host=localhost;Database=HomeworkAi;Username=postgres;Password=postgres"
  },
  "RedisSettings": {
    "ConnectionString": "localhost:6379,abortConnect=false"
  }
}
```

### Essential Environment Setup
1. PostgreSQL server running on localhost:5432
2. Redis server running on localhost:6379
3. OpenAI API key configured
4. Database schemas: Users, Teaching, Exercises

## Domain Concepts

### Core Entities
- **User** (Accounts): Authentication, roles, profile information
- **School → TeachingClass → Assignment → Exercise** (Teaching): Educational hierarchy
- **OpenForm Exercises** (Exercises): Mail, Essay, SummaryOfText with AI generation
- **Student Answers**: Detailed grading with mistake tracking

### AI Integration
- **Exercise Generation**: OpenAI integration for creating language learning exercises
- **Prompt Validation**: Security checks for prompt injection detection
- **Structured Responses**: JSON schema validation for AI-generated content

### Authentication Flow
- JWT-based authentication with refresh tokens
- Role-based authorization (Student, Teacher, Admin)
- Account activation and password reset workflows
- Multi-tenant support through school-based permissions

## Development Considerations

### Adding New Modules
1. Create module folder structure under `Modules/`
2. Implement `ModuleDbContext` with schema configuration
3. Add service registration extension method
4. Register in `Program.cs` bootstrap
5. Create migrations for new entities

### Cross-Module Dependencies
- Use query handlers in `External/` folders for cross-module data access
- Publish domain events for loose coupling
- Avoid direct entity references between modules

### API Development
- Controllers use service layer, never directly access repositories
- Implement proper error handling with `CustomException`
- Use DTO pattern for all API inputs/outputs
- Apply `[Authorize]` attributes appropriately

### Testing Strategy
- xUnit for unit testing
- Separate test projects per module
- Integration tests should use separate test database
- Mock external services (OpenAI) in tests