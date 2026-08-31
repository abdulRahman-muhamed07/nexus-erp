using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeoTechErp.Application.Common;
using XeoTechErp.Application.Contracts.Orders;
using XeoTechErp.Application.CQRS;
using XeoTechErp.Application.Features.Orders.Commands.CreateOrder;
using XeoTechErp.Application.Features.Orders.Queries.GetOrder;
using XeoTechErp.Application.Features.Orders.Queries.GetOrders;

namespace XeoTechErp.Api.Controllers.Orders;

[ApiController]
[Authorize]
[Route("api/orders")]
public sealed class OrdersController(
    ICommandHandler<CreateOrderCommand, Result<OrderDto>> createOrder,
    IQueryHandler<GetOrderQuery, OrderDto?> getOrder,
    IQueryHandler<GetOrdersQuery, IReadOnlyList<OrderDto>> getOrders) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
        => Ok(await getOrders.HandleAsync(new GetOrdersQuery(), cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await getOrder.HandleAsync(new GetOrderQuery(id), cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [Authorize(Policy = "ManagerOrAdmin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var actor = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name ?? "system";
        var result = await createOrder.HandleAsync(new CreateOrderCommand(request, actor), cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
            : BadRequest(result.Error);
    }
}
