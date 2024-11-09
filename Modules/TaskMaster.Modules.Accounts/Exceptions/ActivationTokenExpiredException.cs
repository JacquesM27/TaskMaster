using TaskMaster.Infrastructure.Exceptions;

namespace TaskMaster.Modules.Accounts.Exceptions;

public sealed class ActivationTokenExpiredException() : CustomException("Given activation token is expired.");