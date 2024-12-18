using TaskMaster.Infrastructure.Exceptions;

namespace TaskMaster.Modules.Accounts.Exceptions;

public sealed class InvalidResetPasswordTokenException() : CustomException("Given reset token is invalid.");