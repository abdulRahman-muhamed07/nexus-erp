using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using Microsoft.EntityFrameworkCore;using XeoTechErp.Api.Data;
namespace XeoTechErp.Api.Controllers.System;
[ApiController,Route("api/audit"),Authorize(Roles="Manager,Administrator")]
public class AuditController(XeoTechDbContext db):ControllerBase{
[HttpGet]public async Task<IActionResult> Get([FromQuery]string? module,[FromQuery]string? user,[FromQuery]int page=1,[FromQuery]int pageSize=50){page=Math.Max(1,page);pageSize=Math.Clamp(pageSize,1,200);var q=db.AuditLog.AsNoTracking();if(!string.IsNullOrWhiteSpace(module))q=q.Where(x=>x.Module==module);if(!string.IsNullOrWhiteSpace(user))q=q.Where(x=>x.User==user);var total=await q.CountAsync();return Ok(new{data=await q.OrderByDescending(x=>x.Time).Skip((page-1)*pageSize).Take(pageSize).ToListAsync(),page,pageSize,total});}
}