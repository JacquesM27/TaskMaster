using TaskMaster.Infrastructure.Exceptions;

namespace TaskMaster.Modules.Accounts.Exceptions;

public sealed class UserNotActiveException(string email)
    : CustomException($"User with email : '{email}' is not active.");