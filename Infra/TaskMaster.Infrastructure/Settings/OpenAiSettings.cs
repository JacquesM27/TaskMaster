namespace TaskMaster.Infrastructure.Settings;

public sealed class OpenAiSettings
{
    public const string SectionName = "openAi";
    public string ApiKey { get; set; } = string.Empty;
}