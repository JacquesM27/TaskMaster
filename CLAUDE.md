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

⚠️ **CRITICAL REQUIREMENT**: The project MUST compile successfully after each phase completion. No phase should be considered complete if the build fails. Each phase should be implemented incrementally with working intermediate states.

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
**Build Status:** ✅ COMPILES SUCCESSFULLY

**Scope:**
- Enhanced event abstractions (IDomainEvent, IIntegrationEvent, EventMetadata)
- Module client base abstractions
- Outbox pattern infrastructure for reliable messaging
- Separated event stores for domain and integration events
- Integration event bus with reliability patterns
- Module contract projects for proper boundaries

**Key Deliverables:**
- `TaskMaster.Abstractions/Events/` - Enhanced event interfaces
- `TaskMaster.Abstractions/Modules/` - Module client base interface
- `TaskMaster.Infrastructure/Events/` - Separated event stores and integration bus
- `TaskMaster.Infrastructure/Outbox/` - Outbox pattern implementation
- Module contract projects for each domain

**Architecture Fix Applied:**
🐛 **Critical Bug Fixed**: Resolved impossible type check in original EventStore where `@event is IDomainEvent` was checked on `IIntegrationEvent` constrained generic. Split into separate stores:
- `IDomainEventStore` for event sourcing within aggregates
- `IIntegrationEventStore` for cross-module communication

**Files Created/Modified:**
```
Infra/TaskMaster.Abstractions/Events/
├── IEvent.cs (enhanced with IDomainEvent, IIntegrationEvent)
├── EventMetadata.cs
├── IEventMigration.cs
├── IEventStore.cs (split into IDomainEventStore + IIntegrationEventStore)
└── IIntegrationEventBus.cs

Infra/TaskMaster.Abstractions/Modules/
└── IModuleClient.cs

Modules/*/TaskMaster.Modules.*.Contracts/
├── Accounts/IAccountsModuleClient.cs + UserDto
├── Exercises/IExercisesModuleClient.cs 
└── Teaching/ITeachingModuleClient.cs + DTOs

Infra/TaskMaster.Infrastructure/
├── Events/EventStore.cs (split into DomainEventStore + IntegrationEventStore)
├── Events/IntegrationEventBus.cs
├── Events/EventVersionManager.cs
├── Outbox/OutboxRepository.cs
├── Outbox/OutboxProcessingService.cs
└── DAL/TaskMasterDbContext.cs (separate tables: DomainEvents, IntegrationEvents, OutboxMessages)
```

**Verification Steps:**
1. ✅ All contract projects compile independently
2. ✅ Infrastructure project compiles with separated event stores
3. ✅ Proper type safety - no impossible type checks
4. ✅ Database context includes separate tables for different event types
5. ✅ Service registration correctly maps interfaces to implementations

#### Phase 2: Enhanced Event Architecture
**Timeline:** 1 week  
**Status:** 🔄 NEXT  
**Build Requirement:** ✅ MUST COMPILE AFTER COMPLETION

**Scope:**
- Event store improvements and optimizations
- Event versioning and migration system
- Enhanced event processing with retries and dead letter queues
- Event replay capabilities for debugging

**Implementation Strategy:**
⚠️ **Incremental Development**: Each sub-task must maintain compilation state
1. Add features one at a time
2. Test compilation after each addition
3. Ensure backward compatibility during transition
4. Use feature flags if necessary for gradual rollout

**Key Tasks:**
1. **Event Store Enhancements** (Incremental)
   - Add event snapshots for performance (compile test required)
   - Implement event archiving strategies (compile test required)
   - Add event replay functionality (compile test required)
   - Create event analytics and monitoring (compile test required)

2. **Event Migration System** (Incremental)
   - Implement event version migration pipeline (compile test required)
   - Add automated migration validation (compile test required)
   - Create migration rollback capabilities (compile test required)
   - Add event schema validation (compile test required)

**Files to Create/Modify:**
```
Infra/TaskMaster.Infrastructure/Events/
├── EventSnapshot.cs
├── EventMigrationPipeline.cs
├── EventReplayService.cs
└── EventAnalyticsService.cs
```

**Verification Requirements:**
- [ ] Each new file compiles independently
- [ ] Integration with existing event stores works
- [ ] No breaking changes to existing interfaces
- [ ] All unit tests pass after each addition
- [ ] Full solution builds successfully

#### Phase 3: Domain Events & Integration Events
**Timeline:** 1 week  
**Status:** ⏳ PENDING  
**Build Requirement:** ✅ MUST COMPILE AFTER COMPLETION

**Scope:**
- Define comprehensive domain events for each module
- Create integration events for cross-module workflows
- Implement event handlers with proper error handling
- Add event-driven workflow orchestration

**Implementation Strategy:**
⚠️ **Module-by-Module Approach**: Implement events incrementally by module
1. Start with one module (e.g., Exercises)
2. Add domain events first, test compilation
3. Add integration events, test compilation
4. Add event handlers, test compilation
5. Repeat for next module
6. Test cross-module integration at the end

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
**Build Requirement:** ✅ MUST COMPILE AFTER COMPLETION

**Scope:**
- Implement concrete module clients
- Add anti-corruption layers for domain protection
- Create client-side caching and resilience patterns
- Add cross-module query optimization

**Implementation Strategy:**
⚠️ **Contract-First Development**: Ensure compilation at each step
1. Implement one module client at a time
2. Start with simplest client (e.g., Accounts)
3. Add basic implementation, test compilation
4. Add caching layer, test compilation
5. Add anti-corruption layer, test compilation
6. Add resilience patterns, test compilation
7. Integration test with actual module
8. Repeat for next module client

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
**Build Requirement:** ✅ MUST COMPILE AFTER COMPLETION

**Scope:**
- Refactor Exercises module to vertical slice architecture
- Eliminate code duplication across exercise types
- Implement feature-based organization
- Add comprehensive validation and business rules

**Implementation Strategy:**
⚠️ **Parallel Development**: Run old and new implementations side-by-side
1. Create new feature slice structure alongside existing code
2. Implement one feature slice, test compilation
3. Add feature flag to switch between old/new implementation
4. Test new implementation thoroughly
5. Migrate one endpoint at a time
6. Remove old implementation once all tests pass
7. Ensure no breaking changes to external contracts

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
**Build Requirement:** ✅ MUST COMPILE AFTER COMPLETION

**Scope:**
- Implement complex workflow orchestration
- Add saga pattern for long-running processes
- Create module orchestrator for complex operations
- Add comprehensive monitoring and observability

**Implementation Strategy:**
⚠️ **Conservative Integration**: Test extensively before deployment
1. Implement saga infrastructure first, test compilation
2. Add simple saga (single workflow), test compilation
3. Add orchestrator infrastructure, test compilation
4. Add one complex workflow, test compilation
5. Add monitoring and health checks, test compilation
6. Add comprehensive error handling and retries
7. Full integration testing across all modules
8. Performance testing under load

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

### Compilation Requirements & Verification

**Mandatory Build Verification After Each Phase:**

1. **Individual Project Compilation**
   ```bash
   # Test each modified project individually
   dotnet build Infra/TaskMaster.Abstractions/TaskMaster.Abstractions.csproj
   dotnet build Infra/TaskMaster.Infrastructure/TaskMaster.Infrastructure.csproj
   dotnet build Modules/Exercises/TaskMaster.Modules.Exercises/TaskMaster.Modules.Exercises.csproj
   # ... for each modified project
   ```

2. **Full Solution Compilation**
   ```bash
   # Test entire solution
   dotnet build TaskMaster.sln
   ```

3. **Contract Project Verification**
   ```bash
   # Test all contract projects compile independently
   dotnet build Modules/Exercises/TaskMaster.Modules.Exercises.Contracts/
   dotnet build Modules/Accounts/TaskMaster.Modules.Accounts.Contracts/
   dotnet build Modules/Teaching/TaskMaster.Modules.Teaching.Contracts/
   ```

4. **Test Project Compilation**
   ```bash
   # Verify test projects still compile
   dotnet build Tests/Modules/Users/TaskMaster.Modules.Accounts.Tests.Unit/
   # Note: Some test projects may have package versioning issues - acceptable if main projects compile
   ```

**Rollback Policy:**
- If any phase results in compilation failure, immediately rollback changes
- Fix compilation issues before proceeding to next sub-task
- Never commit code that doesn't compile
- Use feature branches for experimental changes

**Integration Testing Requirements:**
- After each phase completion, run integration tests
- Verify existing functionality still works
- Test new features in isolation
- Validate cross-module communication patterns

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