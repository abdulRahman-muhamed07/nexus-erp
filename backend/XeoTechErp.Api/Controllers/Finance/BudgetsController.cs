using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeoTechErp.Application.Features.Finance;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Api.Controllers.Finance;

[ApiController, Route("api/budgets"), Authorize]
public sealed class BudgetsController(IFinanceService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken) => Ok(await service.GetBudgetsAsync(cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Upsert(Budget input, CancellationToken cancellationToken) => Ok(await service.UpsertBudgetAsync(input, cancellationToken));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await service.DeleteBudgetAsync(id, cancellationToken);
        return NoContent();
    }
}
