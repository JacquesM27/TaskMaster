using TaskMaster.Abstractions.Modules;

namespace TaskMaster.Modules.Accounts.Contracts;

public interface IAccountsModuleClient : IModuleClient
{
    Task<UserDto?> GetUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> IsUserActiveAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
}

public sealed record UserDto(
    Guid Id,
    string Email,
    string Role,
    bool IsActive,
    DateTime CreatedAt);