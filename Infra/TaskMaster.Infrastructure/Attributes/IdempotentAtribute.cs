using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

namespace TaskMaster.Infrastructure.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public sealed class IdempotentAtribute : Attribute, IAsyncActionFilter
{
    private const int DefaultCacheTimeInMinutes = 60;
    private readonly TimeSpan _cacheDuration;

    public IdempotentAtribute(int cacheTimeInMinutes = DefaultCacheTimeInMinutes)
    {
        _cacheDuration = TimeSpan.FromMinutes(cacheTimeInMinutes);
    }
    
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("Idempotence-Key", out var idempotenceKeyValue)
            || !Guid.TryParse(idempotenceKeyValue, out var idempotenceKey))
        {
            context.Result = new BadRequestObjectResult("Invalid or missing Idempotence-Key header");
            return;
        }

        var cache = context.HttpContext.RequestServices.GetRequiredService<IDistributedCache>();
        
        var cacheKey = $"Idempotent_{idempotenceKey}";
        var cacheResult = await cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrWhiteSpace(cacheResult))
        {
            var response = JsonSerializer.Deserialize<IdempotentResponse>(cacheResult)!;
            
            var result = new ObjectResult(response.Value)
            {
                StatusCode = response.StatusCode
            };
            context.Result = result;
            return;
        }
        
        var executedContext = await next();
        
        if (executedContext.Result is ObjectResult { StatusCode: >= 200 and < 300 } objectResult)
        {
            var statusCode = objectResult.StatusCode ?? StatusCodes.Status200OK;
            var response = new IdempotentResponse(statusCode, objectResult.Value);
            
            await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(response), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheDuration
            });
        }
    }
}

[method: JsonConstructor]
public sealed class IdempotentResponse(int statusCode, object? value)
{
    
    /*
     [JsonConstructor]
    public IdempotentResponse(int statusCode, object? value)
    {
        StatusCode = statusCode;
        Value = value;
    }
     */
    public int StatusCode { get; } = statusCode;
    public object? Value { get; } = value;
}