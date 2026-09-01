using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeoTechErp.Application.Services;

namespace XeoTechErp.Api.Controllers.Reports;

[ApiController]
[Route("api/reports")]
[Authorize(Policy = "ManagerOrAdmin")]
public sealed class ReportsController(IReportService service) : ControllerBase
{
    [HttpGet("sales-summary")]
    public async Task<IActionResult> SalesSummary(CancellationToken cancellationToken)
        => Ok(await service.GetSalesSummaryAsync(cancellationToken));
}
