using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TaskMaster.Abstractions.Exceptions;

namespace TaskMaster.Infrastructure.Exceptions;

internal sealed class ErrorHandlerMiddleware(
    IExceptionCompositionRoot exceptionCompositionRoot,
    ILogger<ErrorHandlerMiddleware> logger)
    : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(
                $"There was an error: {exception.GetType().Name}, description: {exception.Message}, stack trace: {exception.StackTrace}");
            await HandleErrorAsync(context, exception);
        }
    }

    private async Task HandleErrorAsync(HttpContext context, Exception exception)
    {
        var errorResponse = exceptionCompositionRoot.Map(exception);
        context.Response.StatusCode = (int)(errorResponse?.StatusCode ?? HttpStatusCode.InternalServerError);
        var response = errorResponse?.Response;
        if (response is null)
            return;
        await context.Response.WriteAsJsonAsync(response);
    }
}