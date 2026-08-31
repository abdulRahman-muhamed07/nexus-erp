using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XeoTechErp.Api.Data;

namespace XeoTechErp.Api.Controllers.Finance;

[ApiController, Route("api/finance"), Authorize]
public sealed class FinanceSummaryController(XeoTechDbContext db) : ControllerBase
{
    [HttpGet("summary")]
    public async Task<IActionResult> Summary(DateTime? from = null, DateTime? to = null)
    {
        var start = from ?? DateTime.UtcNow.Date.AddDays(-30); var end = to ?? DateTime.UtcNow;
        if (start > end) return BadRequest(new { error = "from must be before to" });
        var revenue = await db.Payments.Where(x => x.Date >= start && x.Date <= end).SumAsync(x => (decimal?)x.Amount) ?? 0;
        var returns = await db.Returns.Where(x => x.Date >= start && x.Date <= end).SumAsync(x => (decimal?)x.Amount) ?? 0;
        var expenses = await db.Expenses.Where(x => x.Date >= start && x.Date <= end).SumAsync(x => (decimal?)x.Amount) ?? 0;
        var net = revenue - returns - expenses;
        return Ok(new { from = start, to = end, revenue, returns, expenses, net });
    }
}
