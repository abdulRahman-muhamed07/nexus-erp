using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeoTechErp.Api.DTOs;
using XeoTechErp.Api.Services;
namespace XeoTechErp.Api.Controllers;
[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService auth):ControllerBase
{
 [AllowAnonymous,HttpPost("login")] public async Task<IActionResult> Login(LoginRequest request)=> (await auth.LoginAsync(request)) is { } r?Ok(r):Unauthorized(new{error="Invalid email or password."});
}
