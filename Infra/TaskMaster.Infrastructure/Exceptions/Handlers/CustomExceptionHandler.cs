using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TaskMaster.Infrastructure.Exceptions.Handlers;

internal sealed class CustomExceptionHandler(
    ILogger<CustomExceptionHandler> logger,
    IProblemDetailsService problemDetailsService)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken
        cancellationToken)
    {
        if (exception is not CustomException)
            return false;

        logger.LogError(exception, "Error: {ErrorName}, description: {Message}, stack trace: {StackTrace}",
            exception.GetType().Name, exception.Message, exception.StackTrace);

        var statusCode = exception is CustomException ? StatusCodes.Status400BadRequest : StatusCodes.Status500InternalServerError;
        var problemDetails = new ProblemDetails()
        {
            Status = statusCode,
            Title = "Bad request",
            Detail = exception.Message,
            Type = exception.GetType().Name,Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
        };

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            Exception = exception,
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
        });
    }
}