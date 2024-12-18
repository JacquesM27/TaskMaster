namespace TaskMaster.Modules.Accounts.DTOs;

public sealed record UserDto(
    Guid Id,
    string Email,
    string Role,
    string Firstname,
    string Lastname,
    DateTime CreatedAt,
    Dictionary<string, IEnumerable<string>> Claims);