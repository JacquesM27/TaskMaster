using NSubstitute;
using Shouldly;
using TaskMaster.Modules.Accounts.External.QueryHandlers;
using TaskMaster.Modules.Accounts.Services;
using TasMaster.Queries.Accounts;

namespace TaskMaster.Modules.Accounts.Tests.Unit.QueryHandlers;

public sealed class UserIdByEmailQueryHandlerTests
{
    private readonly UserIdByEmailQueryHandler _handler;
    private readonly IIdentityService _identityService = Substitute.For<IIdentityService>();

    public UserIdByEmailQueryHandlerTests()
    {
        _handler = new UserIdByEmailQueryHandler(_identityService);
    }

    [Fact]
    public async Task HandleAsync_WithValidEmail_ShouldReturnUserId()
    {
        // Arrange
        var email = "test@example.com";
        var expectedUserId = Guid.NewGuid();
        var query = new UserIdByEmailQuery(email);

        _identityService.GetIdByEmailAsync(email).Returns(expectedUserId);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.ShouldBe(expectedUserId);
    }

    [Fact]
    public async Task HandleAsync_WithNonExistentEmail_ShouldReturnNull()
    {
        // Arrange
        var email = "nonexistent@example.com";
        var query = new UserIdByEmailQuery(email);

        _identityService.GetIdByEmailAsync(email).Returns((Guid?)null);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task HandleAsync_WithEmptyEmail_ShouldReturnNull()
    {
        // Arrange
        var email = "";
        var query = new UserIdByEmailQuery(email);

        _identityService.GetIdByEmailAsync(email).Returns((Guid?)null);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.ShouldBeNull();
        await _identityService.Received(1).GetIdByEmailAsync(email);
    }

    [Fact]
    public async Task HandleAsync_WithNullEmail_ShouldCallIdentityServiceAndReturnNull()
    {
        // Arrange
        string email = null!;
        var query = new UserIdByEmailQuery(email);

        _identityService.GetIdByEmailAsync(email).Returns((Guid?)null);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.ShouldBeNull();
        await _identityService.Received(1).GetIdByEmailAsync(email);
    }

    [Fact]
    public async Task HandleAsync_WithWhitespaceEmail_ShouldReturnNull()
    {
        // Arrange
        var email = "   ";
        var query = new UserIdByEmailQuery(email);

        _identityService.GetIdByEmailAsync(email).Returns((Guid?)null);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.ShouldBeNull();
        await _identityService.Received(1).GetIdByEmailAsync(email);
    }

    [Fact]
    public async Task HandleAsync_WithValidEmailVariations_ShouldReturnUserId()
    {
        // Arrange
        var testCases = new[]
        {
            "user@domain.com",
            "User@Domain.COM",
            "user.name@domain.com",
            "user+tag@domain.com",
            "user_name@domain-name.com",
            "123@domain.com",
            "user@domain.co.uk"
        };

        foreach (var email in testCases)
        {
            var expectedUserId = Guid.NewGuid();
            var query = new UserIdByEmailQuery(email);

            _identityService.GetIdByEmailAsync(email).Returns(expectedUserId);

            // Act
            var result = await _handler.HandleAsync(query);

            // Assert
            result.ShouldBe(expectedUserId, $"Failed for email: {email}");
        }
    }

    [Fact]
    public async Task HandleAsync_WithInvalidEmailFormats_ShouldStillPassToService()
    {
        // Arrange - Handler shouldn't validate email format, just pass through
        var invalidEmails = new[]
        {
            "invalid-email",
            "@domain.com",
            "user@",
            "user@@domain.com",
            "user@domain",
            "user space@domain.com"
        };

        foreach (var email in invalidEmails)
        {
            var query = new UserIdByEmailQuery(email);

            _identityService.GetIdByEmailAsync(email).Returns((Guid?)null);

            // Act
            var result = await _handler.HandleAsync(query);

            // Assert
            result.ShouldBeNull();
            await _identityService.Received().GetIdByEmailAsync(email);
        }
    }

    [Fact]
    public async Task HandleAsync_WhenIdentityServiceThrows_ShouldPropagateException()
    {
        // Arrange
        var email = "test@example.com";
        var query = new UserIdByEmailQuery(email);
        var expectedException = new InvalidOperationException("Database connection failed");

        _identityService.GetIdByEmailAsync(email).Returns(Task.FromException<Guid?>(expectedException));

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(
            () => _handler.HandleAsync(query));
        
        exception.Message.ShouldBe("Database connection failed");
    }

    [Fact]
    public async Task HandleAsync_ShouldCallIdentityServiceOnce()
    {
        // Arrange
        var email = "test@example.com";
        var query = new UserIdByEmailQuery(email);

        _identityService.GetIdByEmailAsync(email).Returns((Guid?)null);

        // Act
        await _handler.HandleAsync(query);

        // Assert
        await _identityService.Received(1).GetIdByEmailAsync(email);
    }

    [Fact]
    public async Task HandleAsync_WithLongEmail_ShouldPassThrough()
    {
        // Arrange
        var longEmail = new string('a', 100) + "@" + new string('b', 100) + ".com";
        var expectedUserId = Guid.NewGuid();
        var query = new UserIdByEmailQuery(longEmail);

        _identityService.GetIdByEmailAsync(longEmail).Returns(expectedUserId);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.ShouldBe(expectedUserId);
        await _identityService.Received(1).GetIdByEmailAsync(longEmail);
    }

    [Fact]
    public async Task HandleAsync_WithSpecialCharacterEmails_ShouldPassThrough()
    {
        // Arrange
        var specialEmails = new[]
        {
            "user+tag@domain.com",
            "user.name+tag@domain.com",
            "user_name@domain.com",
            "user-name@domain-name.com",
            "123456@domain.com",
            "user@123.456.789.012"
        };

        foreach (var email in specialEmails)
        {
            var expectedUserId = Guid.NewGuid();
            var query = new UserIdByEmailQuery(email);

            _identityService.GetIdByEmailAsync(email).Returns(expectedUserId);

            // Act
            var result = await _handler.HandleAsync(query);

            // Assert
            result.ShouldBe(expectedUserId, $"Failed for email: {email}");
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("\r\n")]
    public async Task HandleAsync_WithVariousWhitespaceInputs_ShouldReturnNull(string email)
    {
        // Arrange
        var query = new UserIdByEmailQuery(email);

        _identityService.GetIdByEmailAsync(email).Returns((Guid?)null);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.ShouldBeNull();
        await _identityService.Received(1).GetIdByEmailAsync(email);
    }

    [Fact]
    public async Task HandleAsync_MultipleCallsWithSameEmail_ShouldCallServiceEachTime()
    {
        // Arrange
        var email = "test@example.com";
        var expectedUserId = Guid.NewGuid();
        var query = new UserIdByEmailQuery(email);

        _identityService.GetIdByEmailAsync(email).Returns(expectedUserId);

        // Act
        await _handler.HandleAsync(query);
        await _handler.HandleAsync(query);
        await _handler.HandleAsync(query);

        // Assert
        await _identityService.Received(3).GetIdByEmailAsync(email);
    }
}