namespace TaskMaster.Modules.Accounts.DTOs;

public sealed record UserDto(
    Guid Id,
    string Email,
    string Role,
    string FirstName,
    string LastName,
    DateTime CreatedAt,
    Dictionary<string, IEnumerable<string>> Claims,
    string UniqueNumber);