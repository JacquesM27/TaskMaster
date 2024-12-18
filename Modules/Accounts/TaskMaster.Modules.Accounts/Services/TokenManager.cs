using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskMaster.Infrastructure.Settings;
using TaskMaster.Modules.Accounts.DTOs;
using TaskMaster.Modules.Accounts.Exceptions;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace TaskMaster.Modules.Accounts.Services;

internal sealed class TokenManager : ITokenManager
{
    private readonly AuthSettings _authSettings;
    private readonly Dictionary<string, IEnumerable<string>> _emptyClaims = new();
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly SigningCredentials _signingCredentials;
    private readonly TimeProvider _timeProvider;

    public TokenManager(IOptions<AuthSettings> authOptions,
        TimeProvider timeProvider, IRefreshTokenService refreshTokenService)
    {
        if (string.IsNullOrWhiteSpace(authOptions.Value.IssuerSigningKey))
            throw new InvalidOperationException("Issuer signing key is not set.");

        _timeProvider = timeProvider;
        _refreshTokenService = refreshTokenService;
        _signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.Value.IssuerSigningKey)),
            SecurityAlgorithms.HmacSha512);
        _authSettings = authOptions.Value;
    }

    public async Task<JwtDto> CreateTokenAsync(string userId, string email, string? role = null,
        string? audience = null, IDictionary<string, IEnumerable<string>>? claims = null)
    {
        if (string.IsNullOrWhiteSpace(userId) || userId == Guid.Empty.ToString())
            throw new MissingUserIdException();

        var now = _timeProvider.GetLocalNow().DateTime;
        var jwtClaims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.UniqueName, userId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeMilliseconds().ToString())
        };

        if (!string.IsNullOrWhiteSpace(role))
            jwtClaims.Add(new Claim(ClaimTypes.Role, role));

        if (!string.IsNullOrWhiteSpace(audience))
            jwtClaims.Add(new Claim(JwtRegisteredClaimNames.Aud, audience));

        if (claims?.Any() is true)
        {
            var customClaims = new List<Claim>();
            foreach (var (key, values) in claims) customClaims.AddRange(values.Select(value => new Claim(key, value)));
            jwtClaims.AddRange(customClaims);
        }

        var expires = now.Add(_authSettings.Expiry);

        var jwt = new JwtSecurityToken(
            _authSettings.Issuer,
            claims: jwtClaims,
            notBefore: now,
            expires: expires,
            signingCredentials: _signingCredentials);

        var token = new JwtSecurityTokenHandler().WriteToken(jwt);

        var refreshToken = await GenerateRefresh(userId);

        return new JwtDto
        {
            AccessToken = new Token
            {
                GeneratedToken = token,
                TokenExpires = new DateTimeOffset(expires).ToUnixTimeMilliseconds()
            },
            RefreshToken = refreshToken,
            Role = role ?? string.Empty,
            Id = userId,
            Claims = claims ?? _emptyClaims,
            Email = email
        };
    }

    public async Task ValidRefreshAsync(string userId, string refreshToken)
    {
        var storedToken = await _refreshTokenService.GetRefreshTokenAsync(userId);
        if (storedToken != refreshToken)
            throw new InvalidRefreshTokenException();
    }

    public ClaimsPrincipal GetPrincipalFromToken(string token)
    {
        var principal = ValidateToken(token, out var validatedToken);
        if (validatedToken is not JwtSecurityToken jwtSecurityToken
            || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512,
                StringComparison.InvariantCultureIgnoreCase))
            throw new InvalidJwtTokenException();

        return principal;
    }

    private ClaimsPrincipal ValidateToken(string token, out SecurityToken securityToken)
    {
        var validationParams = ValidationParameters();
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParams, out securityToken);
            return principal;
        }
        catch (Exception e)
        {
            throw new InvalidJwtTokenException();
        }
    }

    public Token GenerateActivationToken()
    {
        var expires = _timeProvider.GetLocalNow().DateTime.Add(_authSettings.ActivationExpiry);
        var newToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        return new Token
        {
            GeneratedToken = newToken,
            TokenExpires = new DateTimeOffset(expires).ToUnixTimeMilliseconds()
        };
    }

    private async Task<Token> GenerateRefresh(string userId)
    {
        var expires = _timeProvider.GetLocalNow().DateTime.Add(_authSettings.RefreshExpiry);
        var newToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        await _refreshTokenService.SaveRefreshTokenAsync(userId, newToken, _authSettings.RefreshExpiry);
        return new Token
        {
            GeneratedToken = newToken,
            TokenExpires = new DateTimeOffset(expires).ToUnixTimeMilliseconds()
        };
    }

    private TokenValidationParameters ValidationParameters()
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            RequireAudience = false,
            ValidateActor = false,
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateLifetime = false,
            ValidateTokenReplay = false,
            ValidateIssuerSigningKey = true,
            SaveSigninToken = _authSettings.SaveSigninToken,
            RequireExpirationTime = false,
            RequireSignedTokens = false,
            ClockSkew = TimeSpan.Zero
        };

        if (!string.IsNullOrWhiteSpace(_authSettings.AuthenticationType))
            tokenValidationParameters.AuthenticationType = _authSettings.AuthenticationType;

        var rawKey = Encoding.UTF8.GetBytes(_authSettings.IssuerSigningKey);
        tokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(rawKey);

        if (!string.IsNullOrWhiteSpace(_authSettings.NameClaimType))
            tokenValidationParameters.NameClaimType = _authSettings.NameClaimType;

        if (!string.IsNullOrWhiteSpace(_authSettings.RoleClaimType))
            tokenValidationParameters.RoleClaimType = _authSettings.RoleClaimType;

        return tokenValidationParameters;
    }
}