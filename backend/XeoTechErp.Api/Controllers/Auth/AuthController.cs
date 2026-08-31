using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeoTechErp.Api.Application.Services;
using XeoTechErp.Api.DTOs;

namespace XeoTechErp.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IAuthService auth) : ControllerBase
{
    [AllowAnonymous, HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken) =>
        (await auth.LoginAsync(request, cancellationToken)) is { } response
            ? Ok(response)
            : Unauthorized(new { error = "Invalid email or password." });
}