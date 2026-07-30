using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Services;

namespace MusicStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserService _service) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponseDto>> Register([FromBody] RegisterRequestDto request)
    {
        var result = await _service.RegisterAsync(request);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        var result = await _service.LoginAsync(request);
        return Ok(result);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> RequestPasswordReset([FromBody] ResetPasswordRequestDto request)
    {
        await _service.RequestTokenToResetPasswordAsync(request);
        return Ok();
    }

    [HttpPost("reset-password/confirm")]
    public async Task<IActionResult> ConfirmPasswordReset([FromBody] ResetPasswordDto request)
    {
        await _service.ResetPasswordAsync(request.Email, request.Token, request.NewPassword);
        return Ok();
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
        {
            return Unauthorized();
        }

        await _service.ChangePasswordAsync(email, request.OldPassword, request.NewPassword);
        return Ok();
    }
}