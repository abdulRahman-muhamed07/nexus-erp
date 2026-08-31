using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeoTechErp.Application.Contracts.Finance;
using XeoTechErp.Application.Features.Finance;

namespace XeoTechErp.Api.Controllers.Finance;

[ApiController]
[Route("api/budgets")]
[Authorize]
public sealed class BudgetsController(IFinanceService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken) => Ok(await service.GetBudgetsAsync(cancellationToken));

    [Authorize(Policy = "ManagerOrAdmin")]
    [HttpPost]
    public async Task<IActionResult> Upsert(UpsertBudgetRequest request, CancellationToken cancellationToken) => Ok(await service.UpsertBudgetAsync(request, cancellationToken));

    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await service.DeleteBudgetAsync(id, cancellationToken);
        return NoContent();
    }
}
