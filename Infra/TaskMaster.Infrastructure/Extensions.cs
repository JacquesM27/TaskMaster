using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TaskMaster.Abstractions.Cache;
using TaskMaster.Infrastructure.Auth;
using TaskMaster.Infrastructure.Cache;
using TaskMaster.Infrastructure.Commands;
using TaskMaster.Infrastructure.Events;
using TaskMaster.Infrastructure.Exceptions;
using TaskMaster.Infrastructure.Queries;
using TaskMaster.Infrastructure.Settings;

namespace TaskMaster.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IList<Assembly> assemblies,
        IConfiguration configuration)
    {
        services.Configure<PostgreSqlSettings>(configuration.GetSection(PostgreSqlSettings.SectionName));
        services.Configure<OpenAiSettings>(configuration.GetSection(OpenAiSettings.SectionName));
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<JwtSettings>>().Value);

        services.AddJwtAuth(configuration);
        services.AddJwtInSwagger();
        services.AddErrorHandling();
        services.AddCommands(assemblies);
        services.AddQueries(assemblies);
        services.AddEvents(assemblies);

        services.AddMemoryCache();
        services.AddSingleton<ICacheStorage, CacheStorage>();

        return services;
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseErrorHandling();

        return app;
    }
}