using Microsoft.EntityFrameworkCore;
using TaskMaster.Modules.Accounts.Entities;
using TaskMaster.Modules.Accounts.Repositories;

namespace TaskMaster.Modules.Accounts.DAL.Repositories;

internal sealed class UserRepository(UsersDbContext context) : IUserRepository
{
    private readonly DbSet<User> _users = context.Users;

    public Task<User?> GetAsync(Guid id)
    {
        return _users.SingleOrDefaultAsync(x => x.Id == id);
    }

    public Task<User?> GetAsync(string email)
    {
        return _users.SingleOrDefaultAsync(x => x.Email == email);
    }

    public async Task AddAsync(User user)
    {
        await _users.AddAsync(user);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _users.Update(user);
        await context.SaveChangesAsync();
    }

    public Task<User?> GetByResetPasswordToken(string token)
    {
        return _users.SingleOrDefaultAsync(x => x.PasswordResetToken == token);
    }

    public Task<User?> GetByActivationToken(string token)
    {
        return _users.SingleOrDefaultAsync(x => x.ActivationToken == token);
    }
}