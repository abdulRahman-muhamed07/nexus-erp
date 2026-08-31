using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XeoTechErp.Api.Application.Services;
using XeoTechErp.Api.DTOs;

namespace XeoTechErp.Api.Controllers;

[ApiController, Authorize, Route("api/products")]
public sealed class ProductsController(IProductService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? search, CancellationToken cancellationToken) =>
        Ok(await service.GetAsync(search, cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken) =>
        (await service.GetAsync(id, cancellationToken)) is { } product ? Ok(product) : NotFound();

    [Authorize(Roles = "Manager,Administrator"), HttpPost]
    public async Task<IActionResult> Create(CreateProductRequest request, CancellationToken cancellationToken)
    {
        var product = await service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
    }

    [Authorize(Roles = "Administrator"), HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken) =>
        await service.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
}