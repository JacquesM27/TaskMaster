using TaskMaster.Infrastructure.Exceptions;

namespace TaskMaster.Modules.Accounts.Exceptions;

public sealed class EmailInUseException(string email) : CustomException($"Email '{email}' is already in use.");