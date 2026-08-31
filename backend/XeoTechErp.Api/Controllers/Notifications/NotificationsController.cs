using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XeoTechErp.Api.Data;

namespace XeoTechErp.Api.Controllers.Notifications;

[ApiController, Route("api/notifications"), Authorize]
public sealed class NotificationsController(XeoTechDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(bool unreadOnly = false)
    {
        var q = db.Notifications.AsNoTracking(); if (unreadOnly) q = q.Where(x => !x.IsRead);
        return Ok(await q.OrderByDescending(x => x.Time).Take(100).ToListAsync());
    }

    [HttpPatch("{id:int}/read")]
    public async Task<IActionResult> MarkRead(int id)
    {
        var n = await db.Notifications.FindAsync(id); if (n is null) return NotFound();
        n.IsRead = true; await db.SaveChangesAsync(); return NoContent();
    }

    [HttpPatch("read-all")]
    public async Task<IActionResult> MarkAllRead()
    {
        await db.Notifications.Where(x => !x.IsRead).ExecuteUpdateAsync(s => s.SetProperty(x => x.IsRead, true));
        return NoContent();
    }
}
