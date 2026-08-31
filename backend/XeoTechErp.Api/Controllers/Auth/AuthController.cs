using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeoTechErp.Application.Common;
using XeoTechErp.Application.Contracts.Auth;
using XeoTechErp.Application.Services;

namespace XeoTechErp.Api.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IAuthService auth) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await auth.LoginAsync(request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : Unauthorized(result.Error);
    }
}
