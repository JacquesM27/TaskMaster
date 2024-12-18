using System.ComponentModel.DataAnnotations;

namespace TaskMaster.Modules.Accounts.DTOs;

public sealed class RequestResetPasswordTokenDto
{
    [Required] public string Email { get; init; } = string.Empty;
}