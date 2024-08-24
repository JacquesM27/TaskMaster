using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TaskMaster.Abstractions.Events;

namespace TaskMaster.Infrastructure.Events;

internal static class Extensions
{
    internal static IServiceCollection AddEvents(this IServiceCollection services,
        IEnumerable<Assembly> assemblies)
    {
        // services.AddSingleton<IEventDispatcher, EventDispatcher>();
        //
        // services.Scan(x => x.FromAssemblies(assemblies)
        //     .AddClasses(c => c.AssignableTo(typeof(IEventHandler<>)))
        //     .AsImplementedInterfaces()
        //     .WithScopedLifetime());

        return services;
    }
}