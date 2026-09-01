using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeoTechErp.Application.Features.Finance.Budgets;

namespace XeoTechErp.Api.Controllers.Finance;

[ApiController]
[Route("api/budgets")]
[Authorize]
public sealed class BudgetsController(IBudgetService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
        => Ok(await service.GetAsync(cancellationToken));

    [Authorize(Policy = "ManagerOrAdmin")]
    [HttpPost]
    public async Task<IActionResult> Upsert(UpsertBudgetRequest request, CancellationToken cancellationToken)
    {
        var result = await service.UpsertAsync(request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await service.DeleteAsync(id, cancellationToken);
        return result.IsSuccess ? NoContent() : NotFound(result.Error);
    }
}
