namespace TaskMaster.Infrastructure.Settings;

public sealed class PostgreSqlSettings
{
    public const string SectionName = "postgres";
    public string ConnectionString { get; set; } = string.Empty;
}