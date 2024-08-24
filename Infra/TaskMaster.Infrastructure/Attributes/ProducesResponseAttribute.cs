using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace TaskMaster.Infrastructure.Attributes;

/// <summary>
/// A custom filter that specifies the type of the value and status code returned by the action.
/// Based on Mvc attribute <see cref="ProducesResponseTypeAttribute"/>
/// </summary>
public sealed class ProducesResponseAttribute : ProducesResponseTypeAttribute
{
    public ProducesResponseAttribute(HttpStatusCode statusCode) : base((int)statusCode)
    {
    }

    public ProducesResponseAttribute(HttpStatusCode statusCode, Type type) : base(type, (int)statusCode)
    {
    }

    public ProducesResponseAttribute(int statusCode) : base(statusCode)
    {
    }

    public ProducesResponseAttribute(Type type, int statusCode) : base(type, statusCode)
    {
    }

    public ProducesResponseAttribute(Type type, int statusCode, string contentType,
        params string[] additionalContentTypes) : base(type, statusCode, contentType, additionalContentTypes)
    {
    }
}