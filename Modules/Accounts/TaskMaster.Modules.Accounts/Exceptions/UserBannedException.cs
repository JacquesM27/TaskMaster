using TaskMaster.Infrastructure.Exceptions;

namespace TaskMaster.Modules.Accounts.Exceptions;

public sealed class UserBannedException(string email) : CustomException($"User with email : '{email}' is banned.");