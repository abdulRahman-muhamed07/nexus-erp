using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeoTechErp.Application.Contracts.PurchaseOrders;
using XeoTechErp.Application.Services;
using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Api.Controllers.Procurement;

[ApiController]
[Route("api/purchase-orders")]
[Authorize]
public sealed class PurchaseOrdersController(IPurchaseOrderService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
        => Ok(await service.GetAsync(cancellationToken));

    [Authorize(Policy = "ManagerOrAdmin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreatePurchaseOrderRequest request, CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(request, cancellationToken);
        return result.IsSuccess ? Created($"/api/purchase-orders/{result.Value!.Id}", result.Value) : BadRequest(result.Error);
    }

    [Authorize(Policy = "ManagerOrAdmin")]
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> Status(int id, [FromQuery] PoStatus status, CancellationToken cancellationToken)
    {
        var result = await service.UpdateStatusAsync(id, status, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
