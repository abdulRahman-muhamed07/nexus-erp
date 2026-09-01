using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeoTechErp.Application.Abstractions.Services;

namespace XeoTechErp.Api.Controllers.Inventory;

[ApiController]
[Authorize]
[Route("api/inventory")]
public sealed class InventoryController(IInventoryService service) : ControllerBase
{
    [HttpGet("summary")]
    public async Task<IActionResult> Summary(CancellationToken cancellationToken)
        => Ok(await service.GetSummaryAsync(cancellationToken));

    [Authorize(Roles = "Manager,Administrator")]
    [HttpPost("adjust")]
    public async Task<IActionResult> Adjust(
        int productId,
        int delta,
        string reason = "Manual Adjustment",
        CancellationToken cancellationToken = default)
    {
        var actor = User.FindFirstValue(ClaimTypes.Email) ?? "system";
        var result = await service.AdjustAsync(productId, delta, reason, actor, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}
