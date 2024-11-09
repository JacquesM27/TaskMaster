﻿using System.ComponentModel.DataAnnotations;

namespace TaskMaster.Modules.Accounts.DTOs;

public sealed record SignUpDto
{
    public Guid Id { get; init; }

    [EmailAddress]
    [Required]
    [MaxLength(400)]
    public string Email { get; init; } = string.Empty;

    [Required] [MaxLength(100)] public string Password { get; init; } = string.Empty;

    [Required] [MaxLength(200)] public string Firstname { get; init; } = string.Empty;
    [Required] [MaxLength(200)] public string Lastname { get; init; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    //public Dictionary<string, IEnumerable<string>> Claims { get; init; } = [];
}