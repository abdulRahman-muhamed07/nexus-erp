using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeoTechErp.Application.Features.Finance.Dashboard;

namespace XeoTechErp.Api.Controllers.Finance;

[ApiController]
[Route("api/finance")]
[Authorize]
public sealed class FinanceController(IFinanceReportingService service) : ControllerBase
{
    [HttpGet("summary")]
    public async Task<IActionResult> Summary(CancellationToken cancellationToken)
        => Ok(await service.GetSummaryAsync(cancellationToken));

    [HttpGet("ar-aging")]
    public async Task<IActionResult> Aging(CancellationToken cancellationToken)
        => Ok(await service.GetAgingAsync(cancellationToken));

    [HttpGet("budget-vs-actual")]
    public async Task<IActionResult> Budget(CancellationToken cancellationToken)
        => Ok(await service.GetBudgetVarianceAsync(cancellationToken));

    [HttpGet("period-summary")]
    public async Task<IActionResult> PeriodSummary(
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default)
        => Ok(await service.GetPeriodSummaryAsync(from, to, cancellationToken));
}
