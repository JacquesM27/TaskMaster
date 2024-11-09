using TaskMaster.Infrastructure.Exceptions;

namespace TaskMaster.Modules.Accounts.Exceptions;

public sealed class ResetPasswordTokenExpiredException() : CustomException("Given reset token is expired.");