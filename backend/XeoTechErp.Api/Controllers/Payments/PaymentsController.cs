using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeoTechErp.Application.Contracts.Payments;
using XeoTechErp.Application.Services;

namespace XeoTechErp.Api.Controllers.Payments;

[ApiController]
[Route("api/payments")]
[Authorize]
public sealed class PaymentsController(IPaymentService service) : ControllerBase
{
    [HttpGet("order/{orderId:int}")]
    public async Task<IActionResult> ByOrder(int orderId, CancellationToken cancellationToken)
        => Ok(await service.GetByOrderAsync(orderId, cancellationToken));

    [HttpPost]
    [Authorize(Policy = "ManagerOrAdmin")]
    public async Task<IActionResult> Create(
        CreatePaymentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(request, cancellationToken);
        return result.IsSuccess
            ? Created($"/api/payments/{result.Value!.Id}", result.Value)
            : BadRequest(result.Error);
    }

    [HttpGet("order/{orderId:int}/summary")]
    public async Task<IActionResult> Summary(int orderId, CancellationToken cancellationToken)
    {
        var result = await service.GetSummaryAsync(orderId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }
}
