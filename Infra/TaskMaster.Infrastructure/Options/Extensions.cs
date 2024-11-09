using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskMaster.Infrastructure.Settings;

namespace TaskMaster.Infrastructure.Options;

public static class Extensions
{
    public static T GetOptions<T>(this IServiceCollection services, string sectionName)
        where T : new()
    {
        using var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        return configuration.GetOptions<T>(sectionName);
    }

    public static T GetOptions<T>(this IConfiguration configuration, string sectionName)
        where T : new()
    {
        var options = new T();
        configuration.GetSection(sectionName).Bind(options);
        return options;
    }

    internal static IServiceCollection BindOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .Configure<AuthSettings>(configuration.GetSection(AuthSettings.SectionName))
            .Configure<PostgresSettings>(configuration.GetSection(PostgresSettings.SectionName))
            .Configure<RedisSettings>(configuration.GetSection(RedisSettings.SectionName))
            .Configure<OpenAiSettings>(configuration.GetSection(OpenAiSettings.SectionName));
        return services;
    }
}