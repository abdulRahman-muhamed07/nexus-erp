using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeoTechErp.Application.Contracts.Orders;
using XeoTechErp.Application.Services;

namespace XeoTechErp.Api.Controllers.Orders;

[ApiController]
[Authorize]
[Route("api/orders")]
public sealed class OrdersController(IOrderService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
        => Ok(await service.GetAllAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        => await service.GetAsync(id, cancellationToken) is { } order ? Ok(order) : NotFound();

    [Authorize(Roles = "Manager,Administrator")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var actor = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "system";
        var result = await service.CreateAsync(request, actor, cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { id = result.Value!.Id }, result.Value)
            : BadRequest(result.Error);
    }
}
