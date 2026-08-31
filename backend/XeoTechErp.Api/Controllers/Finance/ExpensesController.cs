using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XeoTechErp.Api.Data;
using XeoTechErp.Api.DTOs;
using XeoTechErp.Api.Models;

namespace XeoTechErp.Api.Controllers.Finance;

[ApiController, Route("api/finance/expenses"), Authorize]
public sealed class ExpensesController(XeoTechDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<Expense>>> Get(int page = 1, int pageSize = 20, string? category = null)
    {
        page = Math.Max(page, 1); pageSize = Math.Clamp(pageSize, 1, 100);
        var q = db.Expenses.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(category)) q = q.Where(x => x.Category == category);
        var total = await q.CountAsync();
        var data = await q.OrderByDescending(x => x.Date).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return Ok(new PagedResponse<Expense>(data, page, pageSize, total));
    }

    [HttpPost]
    [Authorize(Roles = "Manager,Administrator")]
    public async Task<ActionResult<Expense>> Create(ExpenseRequest request)
    {
        if (request.Amount <= 0 || string.IsNullOrWhiteSpace(request.Category)) return BadRequest(new ApiError("INVALID_EXPENSE", "Category and a positive amount are required."));
        var expense = new Expense { Category = request.Category.Trim(), Amount = request.Amount, Date = request.Date == default ? DateTime.UtcNow : request.Date, Description = request.Description?.Trim() ?? "" };
        db.Expenses.Add(expense); await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), null, expense);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(int id)
    {
        var expense = await db.Expenses.FindAsync(id); if (expense is null) return NotFound();
        db.Expenses.Remove(expense); await db.SaveChangesAsync(); return NoContent();
    }
}
