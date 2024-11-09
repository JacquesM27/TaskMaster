namespace TaskMaster.Infrastructure.Settings;

public sealed class OpenAiSettings
{
    public const string SectionName = nameof(OpenAiSettings);
    public string ApiKey { get; set; } = string.Empty;
}