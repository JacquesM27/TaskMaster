using TaskMaster.Infrastructure.Exceptions;

namespace TaskMaster.Modules.Accounts.Exceptions;

public sealed class InvalidActivationTokenException() : CustomException("Given activation token is invalid.");