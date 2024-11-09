using System.ComponentModel.DataAnnotations;

namespace TaskMaster.Modules.Accounts.DTOs;

public sealed class RefreshTokenDto
{
    [Required] public string Jwt { get; init; }
    [Required] public string RefreshToken { get; init; }
}