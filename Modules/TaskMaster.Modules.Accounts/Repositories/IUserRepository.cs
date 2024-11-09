using TaskMaster.Modules.Accounts.Entities;

namespace TaskMaster.Modules.Accounts.Repositories;

public interface IUserRepository
{
    Task<User?> GetAsync(Guid id);
    Task<User?> GetAsync(string email);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task<User?> GetByResetPasswordToken(string token);
    Task<User?> GetByActivationToken(string token);
}