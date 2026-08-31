using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XeoTechErp.Api.Data;
using XeoTechErp.Api.Models;

namespace XeoTechErp.Api.Controllers.Audit;

[ApiController, Route("api/audit"), Authorize(Roles = "Manager,Administrator")]
public sealed class AuditController(XeoTechDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(int page = 1, int pageSize = 25, string? module = null)
    {
        page = Math.Max(page, 1); pageSize = Math.Clamp(pageSize, 1, 100);
        var q = db.AuditLog.AsNoTracking(); if (!string.IsNullOrWhiteSpace(module)) q = q.Where(x => x.Module == module);
        var total = await q.CountAsync(); var data = await q.OrderByDescending(x => x.Time).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return Ok(new { data, page, pageSize, total });
    }
}
