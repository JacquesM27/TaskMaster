namespace TaskMaster.Infrastructure.Settings;

public class PostgresSettings
{
    public const string SectionName = nameof(PostgresSettings);
    public string ConnectionString { get; set; } = string.Empty;
}