using System.Security.Claims;
using TaskMaster.Abstractions.Contexts;

namespace TaskMaster.Infrastructure.Contexts;

internal sealed class IdentityContext : IIdentityContext
{
    public IdentityContext(ClaimsPrincipal principal)
    {
        IsAuthenticated = principal.Identity?.IsAuthenticated is true;
        if (IsAuthenticated)
        {
            var success = Guid.TryParse(principal.Identity?.Name, out var id);
            Id = success ? id : Guid.Empty;
        }

        Role = principal?.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Role)?.Value ?? string.Empty;
        Claims = principal?.Claims
            .GroupBy(x => x.Type)
            .ToDictionary(x => x.Key, x => x.Select(c => c.Value.ToString())) ?? [];
    }

    public bool IsAuthenticated { get; }
    public Guid Id { get; }
    public string Role { get; }
    public Dictionary<string, IEnumerable<string>> Claims { get; }
}