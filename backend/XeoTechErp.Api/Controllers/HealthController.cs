using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XeoTechErp.Api.Data;

namespace XeoTechErp.Api.Controllers;

[ApiController, Route("api/health")]
public sealed class HealthController(XeoTechDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var database = await db.Database.CanConnectAsync();
        return database ? Ok(new { status = "healthy", database = "up", time = DateTime.UtcNow }) : StatusCode(503, new { status = "unhealthy", database = "down", time = DateTime.UtcNow });
    }
}
