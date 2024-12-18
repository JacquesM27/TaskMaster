using TaskMaster.Infrastructure.Exceptions;

namespace TaskMaster.Modules.Accounts.Exceptions;

public sealed class UserNotFoundException : CustomException
{
    public UserNotFoundException(Guid id) : base($"User with id: '{id:N}' was not found.")
    {
    }

    public UserNotFoundException(string email) : base($"User with email: '{email}' was not found.")
    {
    }
}