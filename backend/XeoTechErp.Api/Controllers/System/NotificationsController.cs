using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using Microsoft.EntityFrameworkCore;using XeoTechErp.Api.Data;using XeoTechErp.Api.Models;
namespace XeoTechErp.Api.Controllers.System;
[ApiController,Route("api/notifications"),Authorize]
public class NotificationsController(XeoTechDbContext db):ControllerBase{
[HttpGet]public async Task<IActionResult> Get([FromQuery]bool unreadOnly=false){var q=db.Notifications.AsNoTracking();if(unreadOnly)q=q.Where(x=>!x.IsRead);return Ok(await q.OrderByDescending(x=>x.Time).Take(100).ToListAsync());}
[HttpPatch("{id:int}/read")]public async Task<IActionResult> Read(int id){var n=await db.Notifications.FindAsync(id);if(n is null)return NotFound();n.IsRead=true;await db.SaveChangesAsync();return Ok(n);}
[HttpPost("read-all")]public async Task<IActionResult> ReadAll(){await db.Notifications.Where(x=>!x.IsRead).ExecuteUpdateAsync(x=>x.SetProperty(n=>n.IsRead,true));return NoContent();}
[HttpPost]public async Task<IActionResult> Create(Notification n){if(string.IsNullOrWhiteSpace(n.Title))return BadRequest(new{error="Title is required."});n.Id=0;n.Time=DateTime.UtcNow;db.Notifications.Add(n);await db.SaveChangesAsync();return Created($"/api/notifications/{n.Id}",n);}
}