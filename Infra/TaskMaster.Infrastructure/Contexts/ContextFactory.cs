using Microsoft.AspNetCore.Http;
using TaskMaster.Abstractions.Contexts;

namespace TaskMaster.Infrastructure.Contexts;

internal sealed class ContextFactory(IHttpContextAccessor httpContextAccessor) : IContextFactory
{
    public IContext Create()
    {
        var httpContext = httpContextAccessor.HttpContext;

        return httpContext is null ? Context.Empty : new Context(httpContext);
    }
}