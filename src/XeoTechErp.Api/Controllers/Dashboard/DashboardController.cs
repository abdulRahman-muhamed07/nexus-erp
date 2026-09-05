using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeoTechErp.Application.Abstractions.Services;

namespace XeoTechErp.Api.Controllers.Dashboard;

[ApiController]
[Authorize]
[Route("api/dashboard")]
public sealed class DashboardController(IDashboardService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
        => Ok(await service.GetAsync(cancellationToken));
}
