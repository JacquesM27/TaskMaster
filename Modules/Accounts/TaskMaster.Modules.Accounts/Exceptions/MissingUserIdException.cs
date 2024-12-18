using TaskMaster.Infrastructure.Exceptions;

namespace TaskMaster.Modules.Accounts.Exceptions;

public sealed class MissingUserIdException() : CustomException("User ID claim (subject) cannot be empty");