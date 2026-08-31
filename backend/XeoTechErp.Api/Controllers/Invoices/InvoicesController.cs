using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeoTechErp.Application.Features.Finance.Invoices;
using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Api.Controllers.Invoices;

[ApiController]
[Route("api/invoices")]
[Authorize]
public sealed class InvoicesController(IInvoiceService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] InvoiceStatus? status, CancellationToken cancellationToken)
        => Ok(await service.GetAsync(status, cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [Authorize(Policy = "ManagerOrAdmin")]
    [HttpPost("from-order/{orderId:int}")]
    public async Task<IActionResult> Create(int orderId, CancellationToken cancellationToken)
    {
        var result = await service.CreateFromOrderAsync(orderId, cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
            : BadRequest(result.Error);
    }

    [Authorize(Policy = "ManagerOrAdmin")]
    [HttpPost("{id:int}/pay")]
    public async Task<IActionResult> Pay(int id, CancellationToken cancellationToken)
    {
        var result = await service.PayAsync(id, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
