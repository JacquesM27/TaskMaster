using Microsoft.Extensions.Configuration;

namespace TaskMaster.Infrastructure.Settings;

public static class SettingsExtension
{
    public static T GetConfiguredOptions<T>(this IConfiguration configuration, string sectionName)
        where T : class, new()
    {
        var settings = new T();
        configuration.GetSection(sectionName).Bind(settings);
        return settings;
    }
}