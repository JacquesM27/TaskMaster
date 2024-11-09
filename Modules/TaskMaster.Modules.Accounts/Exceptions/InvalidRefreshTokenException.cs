using TaskMaster.Infrastructure.Exceptions;

namespace TaskMaster.Modules.Accounts.Exceptions;

public sealed class InvalidRefreshTokenException() : CustomException("Invalid refresh token.");