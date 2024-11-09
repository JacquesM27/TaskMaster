using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TaskMaster.Infrastructure.Exceptions.Handlers;

namespace TaskMaster.Infrastructure.Exceptions;

internal static class Extensions
{
    internal static IServiceCollection AddErrorHandling(this IServiceCollection services)
    {
        services.AddExceptionHandler<CustomExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }

    internal static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app)
    {
        app.UseExceptionHandler();

        return app;
    }
}