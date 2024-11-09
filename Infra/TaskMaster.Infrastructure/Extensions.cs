using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskMaster.Abstractions.Cache;
using TaskMaster.Abstractions.Contexts;
using TaskMaster.Infrastructure.Auth;
using TaskMaster.Infrastructure.Cache;
using TaskMaster.Infrastructure.Commands;
using TaskMaster.Infrastructure.Contexts;
using TaskMaster.Infrastructure.Events;
using TaskMaster.Infrastructure.Exceptions;
using TaskMaster.Infrastructure.Options;
using TaskMaster.Infrastructure.Queries;
using TaskMaster.Infrastructure.Redis;

namespace TaskMaster.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IList<Assembly> assemblies,
        IConfiguration configuration)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        services.BindOptions(configuration);
        services.AddSingleton(TimeProvider.System);

        services.AddAuth();
        services.AddUserContext();

        services.AddJwtInSwagger();
        services.AddErrorHandling();
        services.AddCommands(assemblies);
        services.AddQueries(assemblies);
        services.AddEvents(assemblies);

        services.AddMemoryCache();
        services.AddSingleton<ICacheStorage, CacheStorage>();
        services.AddRedis();

        return services;
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseErrorHandling();

        return app;
    }

    private static IServiceCollection AddUserContext(this IServiceCollection services)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
            .AddTransient<IContextFactory, ContextFactory>()
            .AddTransient<IContext>(sp => sp.GetRequiredService<IContextFactory>().Create());
        return services;
    }
}