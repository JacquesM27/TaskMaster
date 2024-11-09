namespace TaskMaster.Modules.Accounts.Services;

public interface IRefreshTokenService
{
    Task SaveRefreshTokenAsync(string userId, string refreshToken, TimeSpan expiryTime);
    Task<string?> GetRefreshTokenAsync(string userId);
}