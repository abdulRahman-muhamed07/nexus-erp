using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeoTechErp.Application.Features.Finance;
using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Api.Controllers.Invoices;

[ApiController, Route("api/invoices"), Authorize]
public sealed class InvoicesController(IFinanceService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] InvoiceStatus? status, CancellationToken cancellationToken) => Ok(await service.GetInvoicesAsync(status, cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken) => Ok(await service.GetInvoiceAsync(id, cancellationToken));

    [HttpPost("from-order/{orderId:int}")]
    public async Task<IActionResult> Create(int orderId, CancellationToken cancellationToken)
    {
        var invoice = await service.CreateInvoiceFromOrderAsync(orderId, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = invoice.Id }, invoice);
    }

    [HttpPost("{id:int}/pay")]
    public async Task<IActionResult> Pay(int id, CancellationToken cancellationToken) => Ok(await service.PayInvoiceAsync(id, cancellationToken));
}
