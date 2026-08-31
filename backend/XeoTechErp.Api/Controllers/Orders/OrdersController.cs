using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using XeoTechErp.Api.Application.Services;
using XeoTechErp.Api.DTOs;

namespace XeoTechErp.Api.Controllers;

[ApiController, Authorize, Route("api/orders")]
public sealed class OrdersController(IOrderService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken) =>
        Ok(await service.GetAllAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken) =>
        (await service.GetAsync(id, cancellationToken)) is { } order ? Ok(order) : NotFound();

    [Authorize(Roles = "Manager,Administrator"), HttpPost]
    public async Task<IActionResult> Create(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var actor = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "system";
        var order = await service.CreateAsync(request, actor, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
    }
}