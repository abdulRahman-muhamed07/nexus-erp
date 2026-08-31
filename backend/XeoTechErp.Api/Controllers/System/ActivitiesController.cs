using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using Microsoft.EntityFrameworkCore;using XeoTechErp.Api.Data;using XeoTechErp.Api.Models;
namespace XeoTechErp.Api.Controllers.System;
[ApiController,Route("api/activities"),Authorize]
public class ActivitiesController(XeoTechDbContext db):ControllerBase{
[HttpGet]public async Task<IActionResult> Get()=>Ok(await db.Activities.AsNoTracking().OrderByDescending(x=>x.Time).Take(100).ToListAsync());
[HttpPost]public async Task<IActionResult> Create(Activity a){if(string.IsNullOrWhiteSpace(a.Text))return BadRequest(new{error="Activity text is required."});a.Id=0;a.Time=DateTime.UtcNow;db.Activities.Add(a);await db.SaveChangesAsync();return Created($"/api/activities/{a.Id}",a);}
}