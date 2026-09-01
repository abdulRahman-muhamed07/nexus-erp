using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeoTechErp.Application.Features.Quotes;
using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Api.Controllers.Quotes;

[ApiController, Route("api/quotes"), Authorize]
public sealed class QuotesController(IQuoteService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] QuoteStatus? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default) => Ok(await service.GetAsync(status, page, pageSize, cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken) => Ok(await service.GetByIdAsync(id, cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create(CreateQuoteRequest request, CancellationToken cancellationToken)
    {
        var quote = await service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = quote.Id }, quote);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> Status(int id, [FromQuery] QuoteStatus status, CancellationToken cancellationToken) => Ok(await service.UpdateStatusAsync(id, status, cancellationToken));

    [HttpPost("{id:int}/convert")]
    public async Task<IActionResult> Convert(int id, CancellationToken cancellationToken) => Ok(await service.ConvertAsync(id, cancellationToken));
}
