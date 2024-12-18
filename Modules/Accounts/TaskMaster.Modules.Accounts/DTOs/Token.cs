namespace TaskMaster.Modules.Accounts.DTOs;

public sealed class Token
{
    public string GeneratedToken { get; init; }
    public long TokenExpires { get; init; }

    public DateTime DateTimeExpires()
    {
        return DateTimeOffset.FromUnixTimeMilliseconds(TokenExpires).DateTime;
    }
}