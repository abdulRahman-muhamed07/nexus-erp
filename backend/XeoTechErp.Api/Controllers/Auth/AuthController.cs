using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using XeoTechErp.Application.Contracts.Auth;
using XeoTechErp.Application.Services;

namespace XeoTechErp.Api.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IAuthService auth) : ControllerBase
{
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await auth.RegisterAsync(request, cancellationToken);
        return result.IsSuccess ? StatusCode(StatusCodes.Status201Created, result.Value) : BadRequest(result.Error);
    }

    [AllowAnonymous]
    [EnableRateLimiting("login")]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await auth.LoginAsync(request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : Unauthorized(result.Error);
    }

    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await auth.RefreshAsync(request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : Unauthorized(result.Error);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await auth.LogoutAsync(request.RefreshToken, cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPut("users/{userId:int}/role")]
    public async Task<IActionResult> UpdateRole(int userId, UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        var result = await auth.UpdateRoleAsync(userId, request, cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}
