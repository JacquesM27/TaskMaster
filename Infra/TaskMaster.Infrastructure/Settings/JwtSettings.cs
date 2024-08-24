﻿namespace TaskMaster.Infrastructure.Settings;

public sealed class JwtSettings
{
    public const string SectionName = "jwt";
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
}