using TaskMaster.Infrastructure.Exceptions;

namespace TaskMaster.Modules.Accounts.Exceptions;

public sealed class InvalidCredentialsException() : CustomException("Invalid credentials.");