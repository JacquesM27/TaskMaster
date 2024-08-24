using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TaskMaster.Abstractions.Exceptions;

namespace TaskMaster.Infrastructure.Exceptions;

internal static class Extensions
{
    internal static IServiceCollection AddErrorHandling(this IServiceCollection services)
    {
        services.AddScoped<ErrorHandlerMiddleware>();
        services.AddSingleton<IExceptionToResponseMapper, ExceptionToResponseMapper>();
        services.AddSingleton<IExceptionCompositionRoot, ExceptionCompositionRoot>();

        return services;
    }

    internal static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app)
    {
        app.UseMiddleware<ErrorHandlerMiddleware>();
        return app;
    }
}