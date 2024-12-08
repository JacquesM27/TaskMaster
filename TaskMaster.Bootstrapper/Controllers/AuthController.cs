using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaskMaster.Bootstrapper.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    [HttpGet]
    [Authorize]
    public IActionResult GetSecureData()
    {
        return Ok("This is a secure data");
    }

    [HttpGet("public")]
    public IActionResult GetUnsecureData()
    {
        return Ok("This is public data");
    }
    //
    // [HttpPost("GetMockToken")]
    // public IActionResult GetMockToken()
    // {
    //     var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
    //     var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    //
    //     var claims = new[]
    //     {
    //         new Claim(JwtRegisteredClaimNames.Sub, "mockUserId"),
    //         new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    //         new Claim(ClaimTypes.Role, "User")
    //     };
    //
    //     var token = new JwtSecurityToken(
    //         issuer: jwtSettings.Issuer,
    //         audience: jwtSettings.Audience,
    //         claims: claims,
    //         expires: DateTime.Now.AddMinutes(30),
    //         signingCredentials: credentials);
    //
    //     var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
    //
    //     return Ok(new { Token = tokenString });
    // }
}