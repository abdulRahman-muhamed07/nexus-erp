using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeoTechErp.Application.Services;

namespace XeoTechErp.Api.Controllers.Audit;

[ApiController]
[Route("api/audit")]
[Authorize(Policy = "ManagerOrAdmin")]
public sealed class AuditController(IAuditService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 25, [FromQuery] string? module = null, CancellationToken cancellationToken = default)
        => Ok(await service.GetAsync(page, pageSize, module, cancellationToken));
}
