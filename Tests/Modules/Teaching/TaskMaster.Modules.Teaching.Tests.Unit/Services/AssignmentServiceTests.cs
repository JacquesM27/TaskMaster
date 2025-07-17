using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;
using TaskMaster.Abstractions.Queries;
using TaskMaster.Models.Exercises.OpenForm;
using TaskMaster.Models.Teaching.Assignment;
using TaskMaster.Models.Teaching.School;
using TaskMaster.Modules.Teaching.Entities;
using TaskMaster.Modules.Teaching.Repositories;
using TaskMaster.Modules.Teaching.Services;
using TasMaster.Queries.Exercises.OpenForm;

namespace TaskMaster.Modules.Teaching.Tests.Services;

public sealed class AssignmentServiceTests
{
    private readonly AssignmentService _service;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IQueryDispatcher _queryDispatcher;

    public AssignmentServiceTests()
    {
        _assignmentRepository = Substitute.For<IAssignmentRepository>();
        _queryDispatcher = Substitute.For<IQueryDispatcher>();
        _service = new AssignmentService(_assignmentRepository, _queryDispatcher);
    }

    #region AddAssignment Tests

    [Fact]
    public async Task AddAssignment_ShouldReturnGuid_WhenValidNewAssignmentProvided()
    {
        // Arrange
        var newAssignment = CreateValidNewAssignmentDto();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _service.AddAssignment(newAssignment, cancellationToken);

        // Assert
        result.ShouldNotBe(Guid.Empty);
        await _assignmentRepository.Received(1).AddAssignmentAsync(
            Arg.Is<Assignment>(a => 
                a.Id == result &&
                a.Name == newAssignment.Name &&
                a.Description == newAssignment.Description &&
                a.Exercises.Count() == newAssignment.Exercises.Count()),
            cancellationToken);
    }

    [Fact]
    public async Task AddAssignment_ShouldMapExercisesCorrectly_WhenMultipleExercisesProvided()
    {
        // Arrange
        var exercises = new List<AssignmentExerciseDto>
        {
            new() { ExerciseId = Guid.NewGuid(), ExerciseType = "Mail" },
            new() { ExerciseId = Guid.NewGuid(), ExerciseType = "Essay" },
            new() { ExerciseId = Guid.NewGuid(), ExerciseType = "Summary" }
        };
        var newAssignment = new NewAssignmentDto
        {
            Name = "Test Assignment",
            Description = "Test Description",
            Exercises = exercises
        };
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _service.AddAssignment(newAssignment, cancellationToken);

        // Assert
        await _assignmentRepository.Received(1).AddAssignmentAsync(
            Arg.Is<Assignment>(a => 
                a.Exercises.Count() == 3 &&
                a.Exercises.All(e => e.Id != Guid.Empty) &&
                a.Exercises.Any(e => e.ExerciseType == "Mail") &&
                a.Exercises.Any(e => e.ExerciseType == "Essay") &&
                a.Exercises.Any(e => e.ExerciseType == "Summary")),
            cancellationToken);
    }

    [Fact]
    public async Task AddAssignment_ShouldGenerateVersion7Guids_WhenCreatingAssignmentAndExercises()
    {
        // Arrange
        var newAssignment = CreateValidNewAssignmentDto();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _service.AddAssignment(newAssignment, cancellationToken);

        // Assert
        result.ShouldNotBe(Guid.Empty);
        await _assignmentRepository.Received(1).AddAssignmentAsync(
            Arg.Is<Assignment>(a => 
                a.Id != Guid.Empty &&
                a.Exercises.All(e => e.Id != Guid.Empty)),
            cancellationToken);
    }

    [Fact]
    public async Task AddAssignment_ShouldHandleEmptyExercisesList_WhenNoExercisesProvided()
    {
        // Arrange
        var newAssignment = new NewAssignmentDto
        {
            Name = "Test Assignment",
            Description = "Test Description",
            Exercises = new List<AssignmentExerciseDto>()
        };
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _service.AddAssignment(newAssignment, cancellationToken);

        // Assert
        result.ShouldNotBe(Guid.Empty);
        await _assignmentRepository.Received(1).AddAssignmentAsync(
            Arg.Is<Assignment>(a => !a.Exercises.Any()),
            cancellationToken);
    }

    [Fact]
    public async Task AddAssignment_ShouldThrowException_WhenRepositoryThrows()
    {
        // Arrange
        var newAssignment = CreateValidNewAssignmentDto();
        var cancellationToken = CancellationToken.None;
        _assignmentRepository.AddAssignmentAsync(Arg.Any<Assignment>(), Arg.Any<CancellationToken>())
            .Throws(new InvalidOperationException("Database error"));

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(() => 
            _service.AddAssignment(newAssignment, cancellationToken));
        exception.Message.ShouldBe("Database error");
    }

    [Fact]
    public async Task AddAssignment_ShouldThrowNullReferenceException_WhenNewAssignmentIsNull()
    {
        // Arrange
        NewAssignmentDto? newAssignment = null;
        var cancellationToken = CancellationToken.None;

        // Act & Assert
        await Should.ThrowAsync<NullReferenceException>(() => 
            _service.AddAssignment(newAssignment!, cancellationToken));
    }

    [Fact]
    public async Task AddAssignment_ShouldThrowArgumentNullException_WhenExercisesCollectionIsNull()
    {
        // Arrange
        var newAssignment = new NewAssignmentDto
        {
            Name = "Test Assignment",
            Description = "Test Description",
            Exercises = null!
        };
        var cancellationToken = CancellationToken.None;

        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(() => 
            _service.AddAssignment(newAssignment, cancellationToken));
    }

    [Fact]
    public async Task AddAssignment_ShouldHandleNullStringProperties_WhenNullValuesProvided()
    {
        // Arrange
        var newAssignment = new NewAssignmentDto
        {
            Name = null!,
            Description = null!,
            Exercises = new List<AssignmentExerciseDto>()
        };
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _service.AddAssignment(newAssignment, cancellationToken);

        // Assert
        result.ShouldNotBe(Guid.Empty);
        await _assignmentRepository.Received(1).AddAssignmentAsync(
            Arg.Is<Assignment>(a => a.Name == null && a.Description == null),
            cancellationToken);
    }

    #endregion

    #region GetAssignmentDetails Tests

    [Fact]
    public async Task GetAssignmentDetails_ShouldReturnAssignmentDetailsDto_WhenAssignmentExists()
    {
        // Arrange
        var assignmentId = Guid.NewGuid();
        var assignment = CreateValidAssignment(assignmentId);
        var cancellationToken = CancellationToken.None;

        _assignmentRepository.GetAssignmentAsync(assignmentId, cancellationToken)
            .Returns(assignment);
        SetupQueryDispatcherMocks();

        // Act
        var result = await _service.GetAssignmentDetails(assignmentId, cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(assignmentId);
        result.Name.ShouldBe(assignment.Name);
        result.Description.ShouldBe(assignment.Description);
        result.MailExercises.ShouldNotBeNull();
        result.EssayExercises.ShouldNotBeNull();
        result.SummaryOfTextExercises.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetAssignmentDetails_ShouldReturnNull_WhenAssignmentDoesNotExist()
    {
        // Arrange
        var assignmentId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        _assignmentRepository.GetAssignmentAsync(assignmentId, cancellationToken)
            .Returns((Assignment?)null);

        // Act
        var result = await _service.GetAssignmentDetails(assignmentId, cancellationToken);

        // Assert
        result.ShouldBeNull();
        await _queryDispatcher.DidNotReceive().QueryAsync(Arg.Any<MailExerciseByIdQuery>());
        await _queryDispatcher.DidNotReceive().QueryAsync(Arg.Any<EssayExerciseByIdQuery>());
        await _queryDispatcher.DidNotReceive().QueryAsync(Arg.Any<SummaryOfTextExerciseByIdQuery>());
    }

    [Fact]
    public async Task GetAssignmentDetails_ShouldQueryCorrectExerciseTypes_WhenMixedExercisesExist()
    {
        // Arrange
        var assignmentId = Guid.NewGuid();
        var mailId = Guid.NewGuid();
        var essayId = Guid.NewGuid();
        var summaryId = Guid.NewGuid();
        
        var assignment = new Assignment
        {
            Id = assignmentId,
            Name = "Test Assignment",
            Description = "Test Description",
            Exercises = new List<AssignmentExercise>
            {
                new() { Id = Guid.NewGuid(), ExerciseId = mailId, ExerciseType = "Mail" },
                new() { Id = Guid.NewGuid(), ExerciseId = essayId, ExerciseType = "Essay" },
                new() { Id = Guid.NewGuid(), ExerciseId = summaryId, ExerciseType = "Summary" }
            }
        };
        var cancellationToken = CancellationToken.None;

        _assignmentRepository.GetAssignmentAsync(assignmentId, cancellationToken)
            .Returns(assignment);
        SetupQueryDispatcherMocks();

        // Act
        var result = await _service.GetAssignmentDetails(assignmentId, cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        await _queryDispatcher.Received(1).QueryAsync(Arg.Is<MailExerciseByIdQuery>(q => q.ExerciseId == mailId));
        await _queryDispatcher.Received(1).QueryAsync(Arg.Is<EssayExerciseByIdQuery>(q => q.ExerciseId == essayId));
        await _queryDispatcher.Received(1).QueryAsync(Arg.Is<SummaryOfTextExerciseByIdQuery>(q => q.ExerciseId == summaryId));
    }

    [Fact]
    public async Task GetAssignmentDetails_ShouldFilterOutNullExerciseResults_WhenSomeExercisesNotFound()
    {
        // Arrange
        var assignmentId = Guid.NewGuid();
        var exerciseId1 = Guid.NewGuid();
        var exerciseId2 = Guid.NewGuid();
        var assignment = new Assignment
        {
            Id = assignmentId,
            Name = "Test Assignment",
            Description = "Test Description",
            Exercises = new List<AssignmentExercise>
            {
                new() { Id = Guid.NewGuid(), ExerciseId = exerciseId1, ExerciseType = "Mail" },
                new() { Id = Guid.NewGuid(), ExerciseId = exerciseId2, ExerciseType = "Mail" }
            }
        };
        var cancellationToken = CancellationToken.None;

        _assignmentRepository.GetAssignmentAsync(assignmentId, cancellationToken)
            .Returns(assignment);
        
        // First query returns a result, second returns null
        _queryDispatcher.QueryAsync(Arg.Is<MailExerciseByIdQuery>(q => q.ExerciseId == exerciseId1))
            .Returns(new MailDto { Id = Guid.NewGuid() });
        _queryDispatcher.QueryAsync(Arg.Is<MailExerciseByIdQuery>(q => q.ExerciseId == exerciseId2))
            .Returns((MailDto?)null);

        // Act
        var result = await _service.GetAssignmentDetails(assignmentId, cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.MailExercises.Count().ShouldBe(1);
        result.EssayExercises.Count().ShouldBe(0);
        result.SummaryOfTextExercises.Count().ShouldBe(0);
    }

    [Fact]
    public async Task GetAssignmentDetails_ShouldHandleEmptyExercisesList_WhenNoExercisesExist()
    {
        // Arrange
        var assignmentId = Guid.NewGuid();
        var assignment = new Assignment
        {
            Id = assignmentId,
            Name = "Test Assignment",
            Description = "Test Description",
            Exercises = new List<AssignmentExercise>()
        };
        var cancellationToken = CancellationToken.None;

        _assignmentRepository.GetAssignmentAsync(assignmentId, cancellationToken)
            .Returns(assignment);

        // Act
        var result = await _service.GetAssignmentDetails(assignmentId, cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.MailExercises.Count().ShouldBe(0);
        result.EssayExercises.Count().ShouldBe(0);
        result.SummaryOfTextExercises.Count().ShouldBe(0);
        await _queryDispatcher.DidNotReceive().QueryAsync(Arg.Any<MailExerciseByIdQuery>());
        await _queryDispatcher.DidNotReceive().QueryAsync(Arg.Any<EssayExerciseByIdQuery>());
        await _queryDispatcher.DidNotReceive().QueryAsync(Arg.Any<SummaryOfTextExerciseByIdQuery>());
    }

    [Fact]
    public async Task GetAssignmentDetails_ShouldThrowException_WhenRepositoryThrows()
    {
        // Arrange
        var assignmentId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;
        _assignmentRepository.GetAssignmentAsync(assignmentId, cancellationToken)
            .Throws(new InvalidOperationException("Database error"));

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(() => 
            _service.GetAssignmentDetails(assignmentId, cancellationToken));
        exception.Message.ShouldBe("Database error");
    }

    [Fact]
    public async Task GetAssignmentDetails_ShouldThrowException_WhenQueryDispatcherThrows()
    {
        // Arrange
        var assignmentId = Guid.NewGuid();
        var assignment = CreateValidAssignment(assignmentId);
        var cancellationToken = CancellationToken.None;

        _assignmentRepository.GetAssignmentAsync(assignmentId, cancellationToken)
            .Returns(assignment);
        _queryDispatcher.QueryAsync(Arg.Any<MailExerciseByIdQuery>())
            .Throws(new InvalidOperationException("Query error"));

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(() => 
            _service.GetAssignmentDetails(assignmentId, cancellationToken));
        exception.Message.ShouldBe("Query error");
    }

    [Fact]
    public async Task GetAssignmentDetails_ShouldHandleUnknownExerciseType_WhenInvalidExerciseTypeProvided()
    {
        // Arrange
        var assignmentId = Guid.NewGuid();
        var assignment = new Assignment
        {
            Id = assignmentId,
            Name = "Test Assignment",
            Description = "Test Description",
            Exercises = new List<AssignmentExercise>
            {
                new() { Id = Guid.NewGuid(), ExerciseType = "UnknownType" }
            }
        };
        var cancellationToken = CancellationToken.None;

        _assignmentRepository.GetAssignmentAsync(assignmentId, cancellationToken)
            .Returns(assignment);

        // Act
        var result = await _service.GetAssignmentDetails(assignmentId, cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.MailExercises.Count().ShouldBe(0);
        result.EssayExercises.Count().ShouldBe(0);
        result.SummaryOfTextExercises.Count().ShouldBe(0);
    }

    #endregion

    #region AddClassAssignment Tests

    [Fact]
    public async Task AddClassAssignment_ShouldReturnGuid_WhenValidNewClassAssignmentProvided()
    {
        // Arrange
        var newClassAssignment = CreateValidNewClassAssignmentDto();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _service.AddClassAssignment(newClassAssignment, cancellationToken);

        // Assert
        result.ShouldNotBe(Guid.Empty);
        await _assignmentRepository.Received(1).AddClassAssignmentAsync(
            Arg.Is<ClassAssignment>(ca => 
                ca.Id == result &&
                ca.AssignmentId == newClassAssignment.AssignmentId &&
                ca.TeachingClassId == newClassAssignment.TeachingClassId &&
                ca.Password == newClassAssignment.Password &&
                ca.DueDate == newClassAssignment.DueDate),
            cancellationToken);
    }

    [Fact]
    public async Task AddClassAssignment_ShouldGenerateVersion7Guid_WhenCreatingClassAssignment()
    {
        // Arrange
        var newClassAssignment = CreateValidNewClassAssignmentDto();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _service.AddClassAssignment(newClassAssignment, cancellationToken);

        // Assert
        result.ShouldNotBe(Guid.Empty);
        await _assignmentRepository.Received(1).AddClassAssignmentAsync(
            Arg.Is<ClassAssignment>(ca => ca.Id != Guid.Empty),
            cancellationToken);
    }

    [Fact]
    public async Task AddClassAssignment_ShouldHandleNullPassword_WhenPasswordIsNull()
    {
        // Arrange
        var newClassAssignment = new NewClassAssignmentDto
        {
            AssignmentId = Guid.NewGuid(),
            TeachingClassId = Guid.NewGuid(),
            DueDate = DateTime.UtcNow.AddDays(7),
            Password = null
        };
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _service.AddClassAssignment(newClassAssignment, cancellationToken);

        // Assert
        result.ShouldNotBe(Guid.Empty);
        await _assignmentRepository.Received(1).AddClassAssignmentAsync(
            Arg.Is<ClassAssignment>(ca => ca.Password == null),
            cancellationToken);
    }

    [Fact]
    public async Task AddClassAssignment_ShouldThrowException_WhenRepositoryThrows()
    {
        // Arrange
        var newClassAssignment = CreateValidNewClassAssignmentDto();
        var cancellationToken = CancellationToken.None;
        _assignmentRepository.AddClassAssignmentAsync(Arg.Any<ClassAssignment>(), Arg.Any<CancellationToken>())
            .Throws(new InvalidOperationException("Database error"));

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(() => 
            _service.AddClassAssignment(newClassAssignment, cancellationToken));
        exception.Message.ShouldBe("Database error");
    }

    [Fact]
    public async Task AddClassAssignment_ShouldThrowNullReferenceException_WhenNewClassAssignmentIsNull()
    {
        // Arrange
        NewClassAssignmentDto? newClassAssignment = null;
        var cancellationToken = CancellationToken.None;

        // Act & Assert
        await Should.ThrowAsync<NullReferenceException>(() => 
            _service.AddClassAssignment(newClassAssignment!, cancellationToken));
    }

    #endregion

    #region AddClassAssignmentWithoutAssignment Tests

    [Fact]
    public async Task AddClassAssignmentWithoutAssignment_ShouldReturnGuid_WhenValidDtoProvided()
    {
        // Arrange
        var newClassAssignment = CreateValidNewClassAssignmentWithoutAssignmentDto();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _service.AddClassAssignmentWithoutAssignment(newClassAssignment, cancellationToken);

        // Assert
        result.ShouldNotBe(Guid.Empty);
        await _assignmentRepository.Received(1).AddAssignmentAsync(Arg.Any<Assignment>(), cancellationToken);
        await _assignmentRepository.Received(1).AddClassAssignmentAsync(Arg.Any<ClassAssignment>(), cancellationToken);
    }

    [Fact]
    public async Task AddClassAssignmentWithoutAssignment_ShouldCreateAssignmentWithCorrectProperties_WhenValidDtoProvided()
    {
        // Arrange
        var newClassAssignment = CreateValidNewClassAssignmentWithoutAssignmentDto();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _service.AddClassAssignmentWithoutAssignment(newClassAssignment, cancellationToken);

        // Assert
        await _assignmentRepository.Received(1).AddAssignmentAsync(
            Arg.Is<Assignment>(a => 
                a.Id != Guid.Empty &&
                a.Description == newClassAssignment.Description &&
                a.Exercises.Count() == newClassAssignment.Exercises.Count()),
            cancellationToken);
    }

    [Fact]
    public async Task AddClassAssignmentWithoutAssignment_ShouldCreateClassAssignmentWithCorrectProperties_WhenValidDtoProvided()
    {
        // Arrange
        var newClassAssignment = CreateValidNewClassAssignmentWithoutAssignmentDto();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _service.AddClassAssignmentWithoutAssignment(newClassAssignment, cancellationToken);

        // Assert
        await _assignmentRepository.Received(1).AddClassAssignmentAsync(
            Arg.Is<ClassAssignment>(ca => 
                ca.Id == result &&
                ca.AssignmentId != Guid.Empty &&
                ca.TeachingClassId == newClassAssignment.TeachingClassId &&
                ca.Password == newClassAssignment.Password &&
                ca.DueDate == newClassAssignment.DueDate),
            cancellationToken);
    }

    [Fact]
    public async Task AddClassAssignmentWithoutAssignment_ShouldMapExercisesCorrectly_WhenMultipleExercisesProvided()
    {
        // Arrange
        var exercises = new List<AssignmentExerciseDto>
        {
            new() { ExerciseId = Guid.NewGuid(), ExerciseType = "Mail" },
            new() { ExerciseId = Guid.NewGuid(), ExerciseType = "Essay" }
        };
        var newClassAssignment = new NewClassAssignmentWithoutAssignmentDto
        {
            Name = "Test Assignment",
            Description = "Test Description",
            TeachingClassId = Guid.NewGuid(),
            DueDate = DateTime.UtcNow.AddDays(7),
            Password = "test123",
            Exercises = exercises
        };
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _service.AddClassAssignmentWithoutAssignment(newClassAssignment, cancellationToken);

        // Assert
        await _assignmentRepository.Received(1).AddAssignmentAsync(
            Arg.Is<Assignment>(a => 
                a.Exercises.Count() == 2 &&
                a.Exercises.All(e => e.Id != Guid.Empty)),
            cancellationToken);
    }

    [Fact]
    public async Task AddClassAssignmentWithoutAssignment_ShouldHandleEmptyExercisesList_WhenNoExercisesProvided()
    {
        // Arrange
        var newClassAssignment = new NewClassAssignmentWithoutAssignmentDto
        {
            Name = "Test Assignment",
            Description = "Test Description",
            TeachingClassId = Guid.NewGuid(),
            DueDate = DateTime.UtcNow.AddDays(7),
            Password = "test123",
            Exercises = new List<AssignmentExerciseDto>()
        };
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _service.AddClassAssignmentWithoutAssignment(newClassAssignment, cancellationToken);

        // Assert
        await _assignmentRepository.Received(1).AddAssignmentAsync(
            Arg.Is<Assignment>(a => !a.Exercises.Any()),
            cancellationToken);
    }

    [Fact]
    public async Task AddClassAssignmentWithoutAssignment_ShouldThrowException_WhenAssignmentRepositoryThrows()
    {
        // Arrange
        var newClassAssignment = CreateValidNewClassAssignmentWithoutAssignmentDto();
        var cancellationToken = CancellationToken.None;
        _assignmentRepository.AddAssignmentAsync(Arg.Any<Assignment>(), Arg.Any<CancellationToken>())
            .Throws(new InvalidOperationException("Database error"));

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(() => 
            _service.AddClassAssignmentWithoutAssignment(newClassAssignment, cancellationToken));
        exception.Message.ShouldBe("Database error");
    }

    [Fact]
    public async Task AddClassAssignmentWithoutAssignment_ShouldThrowException_WhenClassAssignmentRepositoryThrows()
    {
        // Arrange
        var newClassAssignment = CreateValidNewClassAssignmentWithoutAssignmentDto();
        var cancellationToken = CancellationToken.None;
        _assignmentRepository.AddClassAssignmentAsync(Arg.Any<ClassAssignment>(), Arg.Any<CancellationToken>())
            .Throws(new InvalidOperationException("Database error"));

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(() => 
            _service.AddClassAssignmentWithoutAssignment(newClassAssignment, cancellationToken));
        exception.Message.ShouldBe("Database error");
    }

    [Fact]
    public async Task AddClassAssignmentWithoutAssignment_ShouldHandleNullPassword_WhenPasswordIsNull()
    {
        // Arrange
        var newClassAssignment = CreateValidNewClassAssignmentWithoutAssignmentDto();
        newClassAssignment.Password = null;
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _service.AddClassAssignmentWithoutAssignment(newClassAssignment, cancellationToken);

        // Assert
        await _assignmentRepository.Received(1).AddClassAssignmentAsync(
            Arg.Is<ClassAssignment>(ca => ca.Password == null),
            cancellationToken);
    }

    [Fact]
    public async Task AddClassAssignmentWithoutAssignment_ShouldThrowNullReferenceException_WhenNewClassAssignmentIsNull()
    {
        // Arrange
        NewClassAssignmentWithoutAssignmentDto? newClassAssignment = null;
        var cancellationToken = CancellationToken.None;

        // Act & Assert
        await Should.ThrowAsync<NullReferenceException>(() => 
            _service.AddClassAssignmentWithoutAssignment(newClassAssignment!, cancellationToken));
    }

    [Fact]
    public async Task AddClassAssignmentWithoutAssignment_ShouldThrowArgumentNullException_WhenExercisesCollectionIsNull()
    {
        // Arrange
        var newClassAssignment = CreateValidNewClassAssignmentWithoutAssignmentDto();
        newClassAssignment.Exercises = null!;
        var cancellationToken = CancellationToken.None;

        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(() => 
            _service.AddClassAssignmentWithoutAssignment(newClassAssignment, cancellationToken));
    }

    #endregion

    #region GetClassAssignmentDetails Tests

    [Fact]
    public async Task GetClassAssignmentDetails_ShouldReturnClassAssignmentDetailsDto_WhenClassAssignmentExists()
    {
        // Arrange
        var classAssignmentId = Guid.NewGuid();
        var assignmentId = Guid.NewGuid();
        var classAssignment = new ClassAssignment
        {
            Id = classAssignmentId,
            AssignmentId = assignmentId,
            DueDate = DateTime.UtcNow.AddDays(7),
            Password = "test123",
            TeachingClassId = Guid.NewGuid()
        };
        var assignment = CreateValidAssignment(assignmentId);
        var cancellationToken = CancellationToken.None;

        _assignmentRepository.GetClassAssignmentAsync(classAssignmentId, cancellationToken)
            .Returns(classAssignment);
        _assignmentRepository.GetAssignmentAsync(assignmentId, cancellationToken)
            .Returns(assignment);
        SetupQueryDispatcherMocks();

        // Act
        var result = await _service.GetClassAssignmentDetails(classAssignmentId, cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(classAssignmentId);
        result.DueDate.ShouldBe(classAssignment.DueDate);
        result.Assignment.ShouldNotBeNull();
        result.Assignment.Id.ShouldBe(assignmentId);
    }

    [Fact]
    public async Task GetClassAssignmentDetails_ShouldReturnNull_WhenClassAssignmentDoesNotExist()
    {
        // Arrange
        var classAssignmentId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        _assignmentRepository.GetClassAssignmentAsync(classAssignmentId, cancellationToken)
            .Returns((ClassAssignment?)null);

        // Act
        var result = await _service.GetClassAssignmentDetails(classAssignmentId, cancellationToken);

        // Assert
        result.ShouldBeNull();
        await _assignmentRepository.DidNotReceive().GetAssignmentAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetClassAssignmentDetails_ShouldReturnNullAssignment_WhenAssignmentDoesNotExist()
    {
        // Arrange
        var classAssignmentId = Guid.NewGuid();
        var assignmentId = Guid.NewGuid();
        var classAssignment = new ClassAssignment
        {
            Id = classAssignmentId,
            AssignmentId = assignmentId,
            DueDate = DateTime.UtcNow.AddDays(7),
            Password = "test123",
            TeachingClassId = Guid.NewGuid()
        };
        var cancellationToken = CancellationToken.None;

        _assignmentRepository.GetClassAssignmentAsync(classAssignmentId, cancellationToken)
            .Returns(classAssignment);
        _assignmentRepository.GetAssignmentAsync(assignmentId, cancellationToken)
            .Returns((Assignment?)null);

        // Act
        var result = await _service.GetClassAssignmentDetails(classAssignmentId, cancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Assignment.ShouldBeNull();
    }

    [Fact]
    public async Task GetClassAssignmentDetails_ShouldThrowException_WhenRepositoryThrows()
    {
        // Arrange
        var classAssignmentId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;
        _assignmentRepository.GetClassAssignmentAsync(classAssignmentId, cancellationToken)
            .Throws(new InvalidOperationException("Database error"));

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(() => 
            _service.GetClassAssignmentDetails(classAssignmentId, cancellationToken));
        exception.Message.ShouldBe("Database error");
    }

    [Fact]
    public async Task GetClassAssignmentDetails_ShouldThrowException_WhenGetAssignmentDetailsThrows()
    {
        // Arrange
        var classAssignmentId = Guid.NewGuid();
        var assignmentId = Guid.NewGuid();
        var classAssignment = new ClassAssignment
        {
            Id = classAssignmentId,
            AssignmentId = assignmentId,
            DueDate = DateTime.UtcNow.AddDays(7),
            Password = "test123",
            TeachingClassId = Guid.NewGuid()
        };
        var cancellationToken = CancellationToken.None;

        _assignmentRepository.GetClassAssignmentAsync(classAssignmentId, cancellationToken)
            .Returns(classAssignment);
        _assignmentRepository.GetAssignmentAsync(assignmentId, cancellationToken)
            .Throws(new InvalidOperationException("Database error"));

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(() => 
            _service.GetClassAssignmentDetails(classAssignmentId, cancellationToken));
        exception.Message.ShouldBe("Database error");
    }

    #endregion

    #region Cancellation Token Tests

    [Fact]
    public async Task AddAssignment_ShouldPassCancellationToken_WhenRepositoryMethodCalled()
    {
        // Arrange
        var newAssignment = CreateValidNewAssignmentDto();
        var cancellationToken = CancellationToken.None;

        // Act
        await _service.AddAssignment(newAssignment, cancellationToken);

        // Assert
        await _assignmentRepository.Received(1).AddAssignmentAsync(Arg.Any<Assignment>(), cancellationToken);
    }

    [Fact]
    public async Task GetAssignmentDetails_ShouldPassCancellationToken_WhenRepositoryMethodCalled()
    {
        // Arrange
        var assignmentId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        _assignmentRepository.GetAssignmentAsync(assignmentId, cancellationToken)
            .Returns((Assignment?)null);

        // Act
        await _service.GetAssignmentDetails(assignmentId, cancellationToken);

        // Assert
        await _assignmentRepository.Received(1).GetAssignmentAsync(assignmentId, cancellationToken);
    }

    #endregion

    #region Helper Methods

    private NewAssignmentDto CreateValidNewAssignmentDto()
    {
        return new NewAssignmentDto
        {
            Name = "Test Assignment",
            Description = "Test Description",
            Exercises = new List<AssignmentExerciseDto>
            {
                new() { ExerciseId = Guid.NewGuid(), ExerciseType = "Mail" }
            }
        };
    }

    private NewClassAssignmentDto CreateValidNewClassAssignmentDto()
    {
        return new NewClassAssignmentDto
        {
            AssignmentId = Guid.NewGuid(),
            TeachingClassId = Guid.NewGuid(),
            DueDate = DateTime.UtcNow.AddDays(7),
            Password = "test123"
        };
    }

    private NewClassAssignmentWithoutAssignmentDto CreateValidNewClassAssignmentWithoutAssignmentDto()
    {
        return new NewClassAssignmentWithoutAssignmentDto
        {
            Name = "Test Assignment",
            Description = "Test Description",
            TeachingClassId = Guid.NewGuid(),
            DueDate = DateTime.UtcNow.AddDays(7),
            Password = "test123",
            Exercises = new List<AssignmentExerciseDto>
            {
                new() { ExerciseId = Guid.NewGuid(), ExerciseType = "Mail" }
            }
        };
    }

    private Assignment CreateValidAssignment(Guid id)
    {
        return new Assignment
        {
            Id = id,
            Name = "Test Assignment",
            Description = "Test Description",
            Exercises = new List<AssignmentExercise>
            {
                new() { Id = Guid.NewGuid(), ExerciseId = Guid.NewGuid(), ExerciseType = "Mail" }
            }
        };
    }

    private void SetupQueryDispatcherMocks()
    {
        _queryDispatcher.QueryAsync(Arg.Any<MailExerciseByIdQuery>())
            .Returns(new MailDto { Id = Guid.NewGuid() });
        _queryDispatcher.QueryAsync(Arg.Any<EssayExerciseByIdQuery>())
            .Returns(new EssayDto { Id = Guid.NewGuid() });
        _queryDispatcher.QueryAsync(Arg.Any<SummaryOfTextExerciseByIdQuery>())
            .Returns(new SummaryOfTextDto { Id = Guid.NewGuid() });
    }

    #endregion
}