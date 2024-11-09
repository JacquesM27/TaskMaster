using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using TaskMaster.Infrastructure.Options;
using TaskMaster.Infrastructure.Settings;

namespace TaskMaster.Infrastructure.Redis;

internal static class Extensions
{
    internal static IServiceCollection AddRedis(this IServiceCollection services)
    {
        var options = services.GetOptions<RedisSettings>(RedisSettings.SectionName);

        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(options.ConnectionString));

        return services;
    }
}