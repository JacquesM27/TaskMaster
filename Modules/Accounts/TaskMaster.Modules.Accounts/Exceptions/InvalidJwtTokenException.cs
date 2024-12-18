using TaskMaster.Infrastructure.Exceptions;

namespace TaskMaster.Modules.Accounts.Exceptions;

public sealed class InvalidJwtTokenException() : CustomException("Invalid jwt token.");