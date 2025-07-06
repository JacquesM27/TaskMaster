using NSubstitute;
using Shouldly;
using TaskMaster.Modules.Accounts.DTOs;
using TaskMaster.Modules.Accounts.External.QueryHandlers;
using TaskMaster.Modules.Accounts.Services;
using TasMaster.Queries.Accounts;

namespace TaskMaster.Modules.Accounts.Tests.Unit.QueryHandlers;

public sealed class UserByIdQueryHandlerTests
{
    private readonly UserByIdQueryHandler _handler;
    private readonly IIdentityService _identityService = Substitute.For<IIdentityService>();

    public UserByIdQueryHandlerTests()
    {
        _handler = new UserByIdQueryHandler(_identityService);
    }

    [Fact]
    public async Task HandleAsync_WithValidUserId_ShouldReturnMappedUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new UserByIdQuery(userId);
        
        var userDto = new UserDto(
            Id: userId,
            Email: "test@example.com",
            Role: "Student",
            FirstName: "John",
            LastName: "Doe",
            CreatedAt: DateTime.UtcNow,
            Claims: new Dictionary<string, IEnumerable<string>>(),
            UniqueNumber: "U123456"
        );

        _identityService.GetAsync(userId).Returns(userDto);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(userId);
        result.FirstName.ShouldBe("John");
        result.LastName.ShouldBe("Doe");
        result.Email.ShouldBe("test@example.com");
        result.UniqueNumber.ShouldBe("U123456");
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentUserId_ShouldReturnNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new UserByIdQuery(userId);

        _identityService.GetAsync(userId).Returns((UserDto?)null);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task HandleAsync_WithEmptyGuid_ShouldReturnNull()
    {
        // Arrange
        var query = new UserByIdQuery(Guid.Empty);

        _identityService.GetAsync(Guid.Empty).Returns((UserDto?)null);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.ShouldBeNull();
        await _identityService.Received(1).GetAsync(Guid.Empty);
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityServiceThrows_ShouldPropagateException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new UserByIdQuery(userId);
        var expectedException = new InvalidOperationException("Database connection failed");

        _identityService.GetAsync(userId).Returns(Task.FromException<UserDto?>(expectedException));

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(
            () => _handler.HandleAsync(query));
        
        exception.Message.ShouldBe("Database connection failed");
    }

    [Fact]
    public async Task HandleAsync_WithUserHavingEmptyStrings_ShouldMapCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new UserByIdQuery(userId);
        
        var userDto = new UserDto(
            Id: userId,
            Email: "",
            Role: "",
            FirstName: "",
            LastName: "",
            CreatedAt: DateTime.UtcNow,
            Claims: new Dictionary<string, IEnumerable<string>>(),
            UniqueNumber: ""
        );

        _identityService.GetAsync(userId).Returns(userDto);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(userId);
        result.FirstName.ShouldBe("");
        result.LastName.ShouldBe("");
        result.Email.ShouldBe("");
        result.UniqueNumber.ShouldBe("");
    }

    [Fact]
    public async Task HandleAsync_WithUserHavingSpecialCharacters_ShouldMapCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new UserByIdQuery(userId);
        
        var userDto = new UserDto(
            Id: userId,
            Email: "test+user@example.com",
            Role: "Teacher",
            FirstName: "José",
            LastName: "O'Connor-Smith",
            CreatedAt: DateTime.UtcNow,
            Claims: new Dictionary<string, IEnumerable<string>>(),
            UniqueNumber: "U-123/456"
        );

        _identityService.GetAsync(userId).Returns(userDto);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(userId);
        result.FirstName.ShouldBe("José");
        result.LastName.ShouldBe("O'Connor-Smith");
        result.Email.ShouldBe("test+user@example.com");
        result.UniqueNumber.ShouldBe("U-123/456");
    }

    [Fact]
    public async Task HandleAsync_ShouldCallIdentityServiceOnce()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new UserByIdQuery(userId);

        _identityService.GetAsync(userId).Returns((UserDto?)null);

        // Act
        await _handler.HandleAsync(query);

        // Assert
        await _identityService.Received(1).GetAsync(userId);
    }

    [Fact]
    public async Task HandleAsync_WithComplexUserData_ShouldMapOnlyRequiredFields()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new UserByIdQuery(userId);
        
        var complexClaims = new Dictionary<string, IEnumerable<string>>
        {
            { "permissions", new[] { "read", "write", "admin" } },
            { "roles", new[] { "teacher", "admin" } }
        };

        var userDto = new UserDto(
            Id: userId,
            Email: "complex@example.com",
            Role: "Admin",
            FirstName: "Complex",
            LastName: "User",
            CreatedAt: DateTime.UtcNow.AddDays(-30),
            Claims: complexClaims,
            UniqueNumber: "ADMIN001"
        );

        _identityService.GetAsync(userId).Returns(userDto);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(userId);
        result.FirstName.ShouldBe("Complex");
        result.LastName.ShouldBe("User");
        result.Email.ShouldBe("complex@example.com");
        result.UniqueNumber.ShouldBe("ADMIN001");
    }
}