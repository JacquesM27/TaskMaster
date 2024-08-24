using System.Net;

namespace TaskMaster.Abstractions.Exceptions;

public sealed record ExceptionResponse(object Response, HttpStatusCode StatusCode);