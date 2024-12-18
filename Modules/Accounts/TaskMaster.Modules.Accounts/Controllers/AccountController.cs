using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskMaster.Abstractions.Contexts;
using TaskMaster.Modules.Accounts.DTOs;
using TaskMaster.Modules.Accounts.Services;

namespace TaskMaster.Modules.Accounts.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController(IIdentityService identityService, IContext context) : ControllerBase
{
    [HttpGet("ping")]
    public ActionResult<string> Ping()
    {
        return Ok("Pong");
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserDto?>> GetCurrentUserAsync()
    {
        var user = await identityService.GetAsync(context.Identity.Id);
        if (user is null)
            return NotFound();
        return Ok(user);
    }

    [HttpGet("user/{id:guid}")]
    [Authorize]
    public async Task<ActionResult<UserDto?>> GetUserByIdAsync(Guid id)
    {
        var user = await identityService.GetAsync(id);
        if (user is null)
            return NotFound();
        return Ok(user);
    }

    [HttpPost("sign-up-student")]
    public async Task<ActionResult> SignUpStudentAsync(SignUpDto dto)
    {
        dto.Role = "Student";
        await identityService.SignUpAsync(dto);
        return NoContent();
    }

    [HttpPost("sign-up-teacher")]
    public async Task<ActionResult> SignUpTeacherAsync(SignUpDto dto)
    {
        dto.Role = "Teacher";
        await identityService.SignUpAsync(dto);
        return NoContent();
    }

    [HttpPost("sign-up-admin")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> SignUpAdminAsync(SignUpDto dto)
    {
        dto.Role = "Admin";
        await identityService.SignUpAsync(dto);
        return NoContent();
    }

    [HttpPost("sign-in")]
    public async Task<ActionResult<JwtDto>> SignInAsync(SignInDto dto)
    {
        return Ok(await identityService.SignInAsync(dto));
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult> ChangePasswordAsync(ChangePasswordDto dto)
    {
        dto.UserId = context.Identity.Id;
        await identityService.ChangePasswordAsync(dto);
        return Ok();
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult> RequestResetPasswordTokenAsync(RequestResetPasswordTokenDto dto)
    {
        await identityService.RequestResetPasswordTokenAsync(dto);
        return Ok();
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult> ResetPasswordAsync(ResetPasswordDto dto)
    {
        await identityService.ResetPasswordAsync(dto);
        return Ok();
    }

    [HttpPost("ban/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> BanUserAsync(Guid id)
    {
        await identityService.BanUserAsync(id);
        return Ok();
    }

    [HttpPost("unban/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> UnbanUserAsync(Guid id)
    {
        await identityService.UnbanUserAsync(id);
        return Ok();
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult> RefreshTokenAsync(RefreshTokenDto dto)
    {
        return Ok(await identityService.RefreshTokenAsync(dto));
    }

    [HttpPost("new-activation-token")]
    public async Task<ActionResult> NewActivationTokenAsync(NewActivationTokenDto dto)
    {
        await identityService.NewActivationTokenAsync(dto);
        return Ok();
    }

    [HttpPost("activate")]
    public async Task<ActionResult> ActivateAccountAsync(ActivateAccountDto dto)
    {
        await identityService.ActivateAccountAsync(dto);
        return Ok();
    }
}