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

## Inter-Module Communication Refactoring Plan

This section documents the comprehensive refactoring plan to enhance inter-module communication architecture, moving from basic CQRS to a robust hybrid approach with event sourcing, outbox pattern, and module clients.

### Background & Problem Analysis

**Current Issues Identified:**
- TaskMaster.Modules.Exercises follows Transaction Script anti-pattern with repetitive CRUD operations
- Limited inter-module communication patterns
- No reliable event delivery mechanism
- Missing anti-corruption layers for domain protection
- Lack of proper module boundaries and contracts

**Architectural Goals:**
- Implement reliable inter-module communication
- Add event sourcing and versioning capabilities
- Create proper module contracts and boundaries
- Enable complex workflow orchestration
- Maintain loose coupling while improving reliability

### Implementation Phases Overview

#### Phase 1: Foundation - Enhanced Abstractions & Event Infrastructure ✅ COMPLETED
**Timeline:** 1-2 weeks  
**Status:** ✅ COMPLETED (Dec 2024)

**Scope:**
- Enhanced event abstractions (IDomainEvent, IIntegrationEvent, EventMetadata)
- Module client base abstractions
- Outbox pattern infrastructure for reliable messaging
- Event store for persistence and replay
- Integration event bus with reliability patterns

**Key Deliverables:**
- `TaskMaster.Abstractions/Events/` - Enhanced event interfaces
- `TaskMaster.Abstractions/Modules/` - Module client base interface
- `TaskMaster.Infrastructure/Events/` - Event store and integration bus
- `TaskMaster.Infrastructure/Outbox/` - Outbox pattern implementation
- Module contract projects for each domain

**Files Created/Modified:**
```
Infra/TaskMaster.Abstractions/Events/
├── IEvent.cs (enhanced with IDomainEvent, IIntegrationEvent)
├── EventMetadata.cs
├── IEventMigration.cs
├── IEventStore.cs
└── IIntegrationEventBus.cs

Infra/TaskMaster.Abstractions/Modules/
└── IModuleClient.cs

Modules/*/TaskMaster.Modules.*.Contracts/
├── Accounts/IAccountsModuleClient.cs + UserDto
├── Exercises/IExercisesModuleClient.cs 
└── Teaching/ITeachingModuleClient.cs + DTOs

Infra/TaskMaster.Infrastructure/
├── Events/EventStore.cs
├── Events/IntegrationEventBus.cs
├── Events/EventVersionManager.cs
├── Outbox/OutboxRepository.cs
├── Outbox/OutboxProcessingService.cs
└── DAL/TaskMasterDbContext.cs (infrastructure tables)
```

#### Phase 2: Enhanced Event Architecture
**Timeline:** 1 week  
**Status:** 🔄 NEXT

**Scope:**
- Event store improvements and optimizations
- Event versioning and migration system
- Enhanced event processing with retries and dead letter queues
- Event replay capabilities for debugging

**Key Tasks:**
1. **Event Store Enhancements**
   - Add event snapshots for performance
   - Implement event archiving strategies
   - Add event replay functionality
   - Create event analytics and monitoring

2. **Event Migration System**
   - Implement event version migration pipeline
   - Add automated migration validation
   - Create migration rollback capabilities
   - Add event schema validation

**Files to Create/Modify:**
```
Infra/TaskMaster.Infrastructure/Events/
├── EventSnapshot.cs
├── EventMigrationPipeline.cs
├── EventReplayService.cs
└── EventAnalyticsService.cs
```

#### Phase 3: Domain Events & Integration Events
**Timeline:** 1 week  
**Status:** ⏳ PENDING

**Scope:**
- Define comprehensive domain events for each module
- Create integration events for cross-module workflows
- Implement event handlers with proper error handling
- Add event-driven workflow orchestration

**Key Events to Implement:**

**Exercises Module:**
```csharp
// Domain Events
ExerciseCreated, ExerciseVerified, ExerciseUpdated

// Integration Events  
ExerciseReadyForAssignment, ExerciseAssignedToClass
```

**Teaching Module:**
```csharp
// Domain Events
AssignmentCreated, AssignmentDueDateChanged, StudentSubmissionReceived

// Integration Events
AssignmentPublished, ClassAssignmentCompleted
```

**Accounts Module:**
```csharp
// Domain Events
UserRegistered, UserActivated, PermissionsChanged

// Integration Events
TeacherAssignedToSchool, StudentEnrolledInClass
```

#### Phase 4: Module Client Implementation
**Timeline:** 2 weeks  
**Status:** ⏳ PENDING

**Scope:**
- Implement concrete module clients
- Add anti-corruption layers for domain protection
- Create client-side caching and resilience patterns
- Add cross-module query optimization

**Implementation Strategy:**

**1. Exercises Module Client:**
```csharp
// In Exercises module
public sealed class ExercisesModuleClient : IExercisesModuleClient
{
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly ICacheStorage _cache;
    
    public async Task<MailDto?> GetMailExerciseAsync(Guid exerciseId, CancellationToken cancellationToken)
    {
        // Cache-first approach with fallback to query dispatcher
        var cacheKey = $"exercise:mail:{exerciseId}";
        var cached = await _cache.GetAsync<MailDto>(cacheKey, cancellationToken);
        if (cached != null) return cached;
        
        var result = await _queryDispatcher.QueryAsync(new MailExerciseByIdQuery(exerciseId, cancellationToken));
        if (result != null)
        {
            await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10), cancellationToken);
        }
        return result;
    }
}
```

**2. Anti-Corruption Layers:**
```csharp
// In Teaching module
public sealed class ExerciseDataAdapter
{
    public AssignmentExercise ToAssignmentExercise(MailDto externalDto)
    {
        // Validate and transform external data to domain model
        var exerciseId = ExerciseId.From(externalDto.Id);
        var exerciseType = ExerciseType.Mail;
        
        return new AssignmentExercise(exerciseId, exerciseType, externalDto.VerifiedByTeacher);
    }
}
```

#### Phase 5: Feature Slices Implementation  
**Timeline:** 2-3 weeks  
**Status:** ⏳ PENDING

**Scope:**
- Refactor Exercises module to vertical slice architecture
- Eliminate code duplication across exercise types
- Implement feature-based organization
- Add comprehensive validation and business rules

**Target Structure:**
```
Modules/Exercises/Features/
├── GetExercise/
│   ├── GetExerciseQuery.cs
│   ├── GetExerciseHandler.cs
│   └── GetExerciseValidator.cs
├── CreateExercise/
│   ├── CreateExerciseCommand.cs
│   ├── CreateExerciseHandler.cs
│   └── CreateExerciseValidator.cs
├── VerifyExercise/
│   ├── VerifyExerciseCommand.cs
│   └── VerifyExerciseHandler.cs
└── Common/
    ├── ExerciseMappers.cs
    └── ExerciseValidators.cs
```

**Benefits:**
- Eliminates 80% of code duplication in current service layer
- Improves maintainability and testability
- Makes adding new exercise types trivial
- Enables feature-specific optimizations

#### Phase 6: Cross-Module Integration & Orchestration
**Timeline:** 1-2 weeks  
**Status:** ⏳ PENDING

**Scope:**
- Implement complex workflow orchestration
- Add saga pattern for long-running processes
- Create module orchestrator for complex operations
- Add comprehensive monitoring and observability

**Key Implementations:**

**1. Assignment Creation Saga:**
```csharp
public sealed class CreateAssignmentSaga : ISaga
{
    public async Task HandleAsync(AssignmentCreated @event)
    {
        // Step 1: Validate exercises exist and are verified
        var exercises = await _exercisesClient.GetExercisesForAssignmentAsync(@event.AssignmentId);
        var unverifiedExercises = exercises.Where(e => !e.VerifiedByTeacher).ToList();
        
        if (unverifiedExercises.Any())
        {
            await _eventBus.PublishAsync(new AssignmentCreationBlocked(@event.AssignmentId, "Unverified exercises"));
            return;
        }
        
        // Step 2: Create class assignments for all classes
        await _teachingClient.CreateClassAssignmentsAsync(@event.AssignmentId, exercises);
        
        // Step 3: Notify students and teachers
        await _notificationService.NotifyAssignmentCreatedAsync(@event.AssignmentId);
        
        // Step 4: Schedule due date reminders
        await _schedulerService.ScheduleReminderAsync(@event.AssignmentId, @event.DueDate);
    }
}
```

**2. Module Orchestrator:**
```csharp
public sealed class TeachingWorkflowOrchestrator
{
    public async Task<AssignmentWithMetadataDto> CreateCompleteAssignmentAsync(CreateAssignmentRequest request)
    {
        // Validate teacher permissions
        var teacher = await _accountsClient.GetUserAsync(request.CreatedBy);
        var hasPermission = await _accountsClient.IsUserTeacherInSchoolAsync(teacher.Id, request.SchoolId);
        
        if (!hasPermission) throw new UnauthorizedAccessException();
        
        // Get and validate exercises
        var exercises = await _exercisesClient.GetExercisesForClassLevelAsync(request.ClassLevel);
        var validatedExercises = exercises.Where(e => e.VerifiedByTeacher).ToList();
        
        // Create assignment with transaction
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var assignment = await _teachingClient.CreateAssignmentAsync(request);
            await _teachingClient.AttachExercisesToAssignmentAsync(assignment.Id, validatedExercises);
            
            await transaction.CommitAsync();
            
            // Trigger async workflows
            await _eventBus.PublishAsync(new AssignmentCreated(assignment.Id, request.CreatedBy, request.Title, request.DueDate));
            
            return new AssignmentWithMetadataDto(assignment, validatedExercises, teacher);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
```

### Implementation Guidelines

#### Event Naming Conventions
- **Domain Events**: Past tense, module-specific (e.g., `ExerciseCreated`, `UserActivated`)
- **Integration Events**: Past tense, cross-module (e.g., `ExerciseReadyForAssignment`)
- **Commands**: Imperative (e.g., `CreateExercise`, `VerifyExercise`)
- **Queries**: Descriptive (e.g., `GetExerciseById`, `GetAssignmentsForUser`)

#### Error Handling Strategy
- **Transient Errors**: Automatic retry with exponential backoff
- **Business Errors**: Immediate failure with descriptive messages
- **Integration Errors**: Circuit breaker pattern with fallback strategies
- **Data Errors**: Validation at module boundaries with clear error responses

#### Testing Strategy
- **Unit Tests**: Each feature slice has comprehensive unit tests
- **Integration Tests**: Cross-module communication testing
- **Contract Tests**: Module client interface validation
- **End-to-End Tests**: Complete workflow testing including saga patterns

#### Performance Considerations
- **Caching**: Module clients implement intelligent caching
- **Batching**: Event processing supports batch operations
- **Async Processing**: All cross-module operations are asynchronous
- **Connection Pooling**: Optimize database connections across modules

#### Monitoring & Observability
- **Event Tracing**: All events include correlation IDs
- **Performance Metrics**: Track cross-module call latencies
- **Business Metrics**: Monitor saga completion rates and error rates
- **Health Checks**: Module client health monitoring

### Migration Strategy

1. **Backward Compatibility**: Maintain existing APIs during transition
2. **Incremental Rollout**: Implement features module by module
3. **Feature Flags**: Use feature toggles for gradual activation
4. **Rollback Plans**: Maintain ability to revert to previous patterns
5. **Data Migration**: Plan for event store and outbox table creation

### Success Criteria

**Technical Metrics:**
- 99.9% event delivery reliability
- < 100ms average cross-module call latency
- < 5 minutes saga completion time
- Zero data loss during deployments

**Business Metrics:**
- Reduced development time for new features
- Improved system observability and debugging
- Enhanced scalability for future growth
- Better separation of concerns across domains

**Code Quality Metrics:**
- 80% reduction in code duplication (Exercises module)
- 90%+ test coverage for all new components
- Clear module boundaries with no circular dependencies
- Comprehensive documentation and architectural guidelines