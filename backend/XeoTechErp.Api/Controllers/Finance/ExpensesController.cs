using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeoTechErp.Api.DTOs;
using XeoTechErp.Application.Features.Finance;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Api.Controllers.Finance;

[ApiController, Route("api/finance/expenses"), Authorize]
public sealed class ExpensesController(IFinanceService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<Expense>>> Get(int page = 1, int pageSize = 20, string? category = null, CancellationToken cancellationToken = default)
    {
        var result = await service.GetExpensesAsync(page, pageSize, category, cancellationToken);
        return Ok(new PagedResponse<Expense>(result.Data, Math.Max(page, 1), Math.Clamp(pageSize, 1, 100), result.Total));
    }

    [HttpPost]
    [Authorize(Roles = "Manager,Administrator")]
    public async Task<ActionResult<Expense>> Create(ExpenseRequest request, CancellationToken cancellationToken)
    {
        var expense = new Expense { Category = request.Category, Amount = request.Amount, Date = request.Date, Description = request.Description };
        var created = await service.CreateExpenseAsync(expense, cancellationToken);
        return CreatedAtAction(nameof(Get), null, created);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await service.DeleteExpenseAsync(id, cancellationToken);
        return NoContent();
    }
}
