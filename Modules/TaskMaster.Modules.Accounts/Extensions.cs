using Microsoft.Extensions.DependencyInjection;
using TaskMaster.Infrastructure.DAL;
using TaskMaster.Modules.Accounts.DAL;
using TaskMaster.Modules.Accounts.DAL.Repositories;
using TaskMaster.Modules.Accounts.Repositories;
using TaskMaster.Modules.Accounts.Services;

namespace TaskMaster.Modules.Accounts;

public static class Extensions
{
    public static IServiceCollection AddAccounts(this IServiceCollection services)
    {
        return services
            .AddPostgres<UsersDbContext, DatabaseInitializer>()
            .AddTransient<IIdentityService, IdentityService>()
            .AddSingleton<IPasswordManager, PasswordManager>()
            .AddSingleton<ITokenManager, TokenManager>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddTransient<IRefreshTokenService, RefreshTokenService>();
    }
}