using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeoTechErp.Application.Contracts.Finance;
using XeoTechErp.Application.Features.Finance;

namespace XeoTechErp.Api.Controllers.Finance;

[ApiController]
[Route("api/finance/expenses")]
[Authorize]
public sealed class ExpensesController(IFinanceService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? category = null, CancellationToken cancellationToken = default)
        => Ok(await service.GetExpensesAsync(page, pageSize, category, cancellationToken));

    [Authorize(Policy = "ManagerOrAdmin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateExpenseRequest request, CancellationToken cancellationToken)
        => Created("/api/finance/expenses", await service.CreateExpenseAsync(request, cancellationToken));

    [Authorize(Policy = "AdminOnly")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await service.DeleteExpenseAsync(id, cancellationToken);
        return NoContent();
    }
}
