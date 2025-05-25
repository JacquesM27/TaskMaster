using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using TaskMaster.Infrastructure.Attributes;

namespace TaskMaster.Infrastructure.Filters;

public sealed class IdempotencyFilter(int cacheTimeInMinutes = 60) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("Idempotence-Key", out var idempotenceKeyValue)
            || !Guid.TryParse(idempotenceKeyValue, out var idempotenceKey))
        {
            return Results.BadRequest("Invalid or missing Idempotence-Key header");
        }

        var cache = context.HttpContext.RequestServices.GetRequiredService<IDistributedCache>();
        
        var cacheKey = $"Idempotent_{idempotenceKey}";
        var cacheResult = await cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrWhiteSpace(cacheResult))
        {
            var response = JsonSerializer.Deserialize<IdempotentResponse>(cacheResult)!;
            return new IdempotentResult(response.StatusCode, response.Value);
        }
        
        var result = await next(context);
        
        if (result is IStatusCodeHttpResult { StatusCode: >= 200 and < 300 } statusCodeResult and IValueHttpResult valueResult)
        {
            var statusCode = statusCodeResult.StatusCode ?? StatusCodes.Status200OK;
            var response = new IdempotentResponse(statusCode, valueResult.Value);
            
            await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(response), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheTimeInMinutes)
            });
        }
        
        return result;
    }
}

internal sealed class IdempotentResult : IResult
{
    private readonly int _statusCode;
    private readonly object? _value;

    public IdempotentResult(int statusCode, object? value)
    {
        _statusCode = statusCode;
        _value = value;
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = _statusCode;

        return httpContext.Response.WriteAsJsonAsync(_value);
    }
}