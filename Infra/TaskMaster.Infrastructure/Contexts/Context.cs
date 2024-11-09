using Microsoft.AspNetCore.Http;
using TaskMaster.Abstractions.Contexts;

namespace TaskMaster.Infrastructure.Contexts;

internal sealed class Context : IContext
{
    public Context(HttpContext context) : this(context.TraceIdentifier, new IdentityContext(context.User))
    {
    }

    private Context(string traceId, IIdentityContext identity)
    {
        TraceId = traceId;
        Identity = identity;
    }

    private Context()
    {
    }

    public static IContext Empty => new Context();
    public string RequestId { get; } = $"{Guid.NewGuid():N}";
    public string TraceId { get; }
    public IIdentityContext Identity { get; }
}