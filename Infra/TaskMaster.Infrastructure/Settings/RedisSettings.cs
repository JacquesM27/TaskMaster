namespace TaskMaster.Infrastructure.Settings;

public class RedisSettings
{
    public const string SectionName = nameof(RedisSettings);
    public string ConnectionString { get; set; } = string.Empty;
}