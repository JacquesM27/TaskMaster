using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TaskMaster.Abstractions.Queries;

namespace TaskMaster.Infrastructure.Queries;

internal static class Extensions
{
    internal static IServiceCollection AddQueries(this IServiceCollection services,
        IEnumerable<Assembly> assemblies)
    {
        services.AddSingleton<IQueryDispatcher, QueryDispatcher>();

        services.Scan(x => x.FromAssemblies(assemblies)
            .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
        return services;
    }
}